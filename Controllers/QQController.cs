using OAuthApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace OAuthApp.Controllers
{
    public class QQController : Controller
    {
        readonly HttpContext _httpContext;
        readonly AppDbContext _dbContext;

        const string Authorize_url = "https://graph.qq.com/oauth2.0/authorize";
        const string Access_token_url = "https://graph.qq.com/oauth2.0/token";

        const string UserOpenId_url = "https://graph.qq.com/oauth2.0/me";

        const string UserProfile_url = "https://graph.qq.com/user/get_user_info";
        //需要开通UnionId权限
        const string UserUnionId_url = "https://graph.qq.com/oauth2.0/me?access_token={0}&unionid=1";

        public QQController(IHttpContextAccessor contextAccessor,
            AppDbContext dbContext)
        {
            _httpContext = contextAccessor.HttpContext;
            _dbContext = dbContext;
        }


        [HttpGet]
        public ActionResult Authorize([FromQuery] string code, [FromQuery] long state, [FromQuery] string jxqq)
        {
            var props = _dbContext.PropertySettings
                .Where(x => x.ChannelCode.Equals(ChannelCodes.App) && x.ChannelAppId == state).ToList();

            if (props.Count < 1)
            {
                return NotFound("请先配置应用");
            }

            var QQClientID = props
          .FirstOrDefault(x => x.Name.Equals(PropKeyConst.QQClientID)).Value;

            var QQClientSecret = props
          .FirstOrDefault(x => x.Name.Equals(PropKeyConst.QQClientSecret)).Value;

            var QQRedirectUri = props
          .FirstOrDefault(x => x.Name.Equals(PropKeyConst.QQRedirectUri)).Value;

           
            var host = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host.ToString();

            var redirect_uri = $"{host}/qq/Authorize";

            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(jxqq))
            {
                var queryStrings = $"client_id={QQClientID}&redirect_uri={HttpUtility.UrlEncode(redirect_uri)}&response_type=code&scope=get_user_info&state={state}";

                var redirectTo = Authorize_url + "?" + queryStrings;

                return new RedirectResult(redirectTo);
            }

            else
            {
                var OAuthorizeResult = GetAccessToken(QQClientID, QQClientSecret, code, redirect_uri);

                var redirectTo = QQRedirectUri + $"?id={state}&app_qq={HttpUtility.UrlEncode(OAuthorizeResult)}";

                return new RedirectResult(redirectTo);
            }
        }

        private static string GetAccessToken(string client_id, string client_secret, string code, string redirect_uri)
        {
            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = $"{Access_token_url}?client_id={client_id}&client_secret={client_secret}&grant_type=authorization_code&redirect_uri={redirect_uri}&code={code}";

                result = hc.GetStringAsync(url).Result;
            }

            string access_token;
            try
            {
                access_token = result.Split('&')[0].Replace("access_token=", "");
            }
            catch { return result; }

            using (var hc = new HttpClient())
            {
                result = hc.GetStringAsync(UserOpenId_url + "?access_token=" + access_token).Result;
            }

            string userOpeid;
            try
            {
                userOpeid = result.Substring(result.IndexOf("openid") + 9);
                userOpeid = userOpeid.Substring(0, userOpeid.IndexOf("\""));
            }
            catch
            {
                return result;
            }

            using (var hc = new HttpClient())
            {
                var url = $"{UserProfile_url}?access_token={access_token}&openid={userOpeid}&oauth_consumer_key={client_id}";

                result = hc.GetStringAsync(url).Result;
            }

            var responseJson = JsonConvert.DeserializeObject<JObject>(result);

            #region 报错
            if (responseJson["ret"] != null && responseJson["ret"].Value<string>() != "0")
            {
                return responseJson["msg"].Value<string>();
            }
            #endregion


            using (var hc = new HttpClient())
            {
                var url = string.Format(UserUnionId_url, access_token);

                result = hc.GetStringAsync(url).Result;
            }

            var UserUnionId = string.Empty;

            try
            {
                UserUnionId = result.Split("\"unionid\":\"")[1].Split("\"")[0];
            }
            catch
            {

            }

            return JsonConvert.SerializeObject(new
            {
                access_token,
                openid = userOpeid,
                unionId = UserUnionId,
                location = responseJson["province"].Value<string>() + "-" + responseJson["city"].Value<string>(),
                avator = responseJson["figureurl_qq_2"].Value<string>(),
                gender = responseJson["gender"].Value<string>(),
                nickname = responseJson["nickname"].Value<string>(),
            });
        }

    }
}