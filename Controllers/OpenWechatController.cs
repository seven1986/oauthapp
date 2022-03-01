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
    public class OpenWechatController : Controller
    {
        readonly HttpContext _httpContext;
        readonly AppDbContext _dbContext;

        // 微信开放平台网站应用专用
        const string Authorize_url = "https://open.weixin.qq.com/connect/qrconnect";

        public OpenWechatController(IHttpContextAccessor contextAccessor,
            AppDbContext dbContext)
        {
            _httpContext = contextAccessor.HttpContext;
            _dbContext = dbContext;
        }

        public const string default_scope = "snsapi_login";

        [HttpGet]
        public ActionResult Authorize([FromQuery] string code, [FromQuery] long state, [FromQuery] string jxwechat)
        {
            var props = _dbContext.PropertySettings
                .Where(x => x.ChannelCode.Equals(ChannelCodes.App) && x.ChannelAppId == state).ToList();

            if (props.Count < 1)
            {
                return NotFound("请先配置应用");
            }

            var OpenWechatClientID = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.OpenWechatClientID)).Value;

            //if (string.IsNullOrWhiteSpace(OpenWechatClientID))
            //{
            //    OpenWechatClientID = default_client_id;
            //}

            var OpenWechatClientSecret = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.OpenWechatClientSecret)).Value;

            //if (string.IsNullOrWhiteSpace(OpenWechatClientSecret))
            //{
            //    OpenWechatClientSecret = default_client_secret;
            //}

            var OpenWechatScope = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.OpenWechatScope)).Value;

            //if (string.IsNullOrWhiteSpace(OpenWechatScope))
            //{
            //    OpenWechatScope = default_scope;
            //}

            var OpenWechatRedirectUri = props
               .FirstOrDefault(x => x.Name.Equals(PropKeyConst.OpenWechatRedirectUri)).Value;

            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(jxwechat))
            {
                var host = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host.ToString();

                var redirect_uri = $"{host}/OpenWechat/Authorize";

                var queryStrings = $"appid={OpenWechatClientID}&redirect_uri={redirect_uri}&response_type=code&scope={OpenWechatScope}&state={state}#wechat_redirect";

                var redirectTo = Authorize_url + "?" + queryStrings;

                return new RedirectResult(redirectTo);
            }

            else
            {
                var OAuthorizeResult = GetAccessToken(OpenWechatClientID, OpenWechatClientSecret, code, OpenWechatScope);

                var redirectTo = OpenWechatRedirectUri + $"?id={state}&app_wechat={HttpUtility.UrlEncode(OAuthorizeResult)}";

                return new RedirectResult(redirectTo);
            }
        }

        private string GetAccessToken(string client_id, string client_secret, string code, string scope)
        {
            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={client_id}&secret={client_secret}&code={code}&grant_type=authorization_code";

                result = hc.GetStringAsync(url).Result;
            }

            var responseJson = JsonConvert.DeserializeObject<JObject>(result);

            #region 报错
            if (responseJson["errcode"] != null && responseJson["errcode"].HasValues)
            {
                return responseJson["errcode"].Value<string>();
            }
            #endregion

            #region 如果scope是snsapi_userinfo 
            if (scope.Equals(default_scope))
            {
                var access_token = responseJson["access_token"].Value<string>();

                var openid = responseJson["openid"].Value<string>();

                using (var hc = new HttpClient())
                {
                    var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={access_token}&openid={openid}&lang=zh_CN";

                    result = hc.GetStringAsync(url).Result;
                }

                responseJson = JsonConvert.DeserializeObject<JObject>(result);

                #region 报错
                if (responseJson["errcode"] != null && responseJson["errcode"].HasValues)
                {
                    return responseJson["errcode"].Value<string>();
                }
                #endregion

                responseJson.Add("access_token", access_token);

                return JsonConvert.SerializeObject(responseJson);
            }
            #endregion

            //scope是snsapi_base
            return result;
        }

    }
}