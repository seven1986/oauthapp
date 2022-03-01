using OAuthApp.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace OAuthApp.Controllers
{
    public class WeiboController : Controller
    {
        readonly HttpContext _httpContext;
        readonly AppDbContext _dbContext;

        const string Authorize_url = "https://api.weibo.com/oauth2/authorize";
        const string Access_token_url = "https://api.weibo.com/oauth2/access_token";
        const string UserProfile_url = "https://api.weibo.com/2/users/show.json";

        public WeiboController(IHttpContextAccessor contextAccessor,
            AppDbContext dbContext)
        {
            _httpContext = contextAccessor.HttpContext;
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult Authorize([FromQuery] string code, [FromQuery] long state, [FromQuery] string jxweibo)
        {
            var props = _dbContext.PropertySettings
                .Where(x => x.ChannelCode.Equals(ChannelCodes.App) && x.ChannelAppId == state).ToList();

            if (props.Count < 1)
            {
                return NotFound("请先配置应用");
            }

            var WeiboClientID = props
               .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WeiboClientID)).Value;
            var WeiboClientSecret = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WeiboClientSecret)).Value;
            var WeiboRedirectUri = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WeiboRedirectUri)).Value;

            var host = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host.ToString();

            var redirect_uri = $"{host}/Weibo/Authorize";

            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(jxweibo))
            {
                var queryStrings = $"client_id={WeiboClientID}&redirect_uri={redirect_uri}&response_type=code&state={state}";

                var redirectTo = Authorize_url + "?" + queryStrings;

                return new RedirectResult(redirectTo);
            }

            else
            {
                var OAuthorizeResult = GetAccessToken(WeiboClientID, WeiboClientSecret, code, redirect_uri);

                var redirectTo = WeiboRedirectUri + $"?id={state}&app_weibo={HttpUtility.UrlEncode(OAuthorizeResult)}";

                return new RedirectResult(redirectTo);
            }
        }

        private static string GetAccessToken(string client_id, string client_secret, string code, string redirect_uri)
        {
            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = $"{Access_token_url}?client_id={client_id}&client_secret={client_secret}&grant_type=authorization_code&redirect_uri={redirect_uri}&code={code}";

                var response = hc.PostAsync(url, new StringContent(string.Empty)).Result;

                result = response.Content.ReadAsStringAsync().Result;
            }

            //var logFilePath = _hostingEnvironment.WebRootPath + "\\log.txt";

            //using (var sw = new StreamWriter(logFilePath,true,Encoding.UTF8))
            //{
            //    sw.WriteLine("获取token：");
            //    sw.WriteLine(result);
            //}

            var responseJson = JsonConvert.DeserializeObject<JObject>(result);

            #region 报错
            if (responseJson["error"] != null && responseJson["error"].HasValues)
            {
                return responseJson["error_description"].Value<string>();
            }
            #endregion

            var access_token = responseJson["access_token"].Value<string>();

            var openid = responseJson["uid"].Value<string>();

            using (var hc = new HttpClient())
            {
                var url = $"{UserProfile_url}?access_token={access_token}&uid={openid}";

                result = hc.GetStringAsync(url).Result;
            }

            //using (var sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
            //{
            //    sw.WriteLine("获取用户信息：");
            //    sw.WriteLine(result);
            //}

            responseJson = JsonConvert.DeserializeObject<JObject>(result);

            #region 报错
            if (responseJson["error"] != null && responseJson["error"].HasValues)
            {
                return responseJson["error_description"].Value<string>();
            }
            #endregion

            return JsonConvert.SerializeObject(new
            {
                access_token,
                openid,
                location = responseJson["location"].Value<string>(),
                avator = responseJson["profile_image_url"].Value<string>(),
                gender = responseJson["gender"].Value<string>(),
                nickname = responseJson["screen_name"].Value<string>(),
            });
        }

    }
}