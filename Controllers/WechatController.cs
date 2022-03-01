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
    public class WechatController : Controller
    {
        readonly HttpContext _httpContext;
        readonly AppDbContext _dbContext;

        const string Authorize_url = "https://open.weixin.qq.com/connect/oauth2/authorize";

        // 开放平台网站应用专用
        const string Authorize_url2 = "https://open.weixin.qq.com/connect/qrconnect";

        public WechatController(IHttpContextAccessor contextAccessor,
            AppDbContext dbContext)
        {
            _httpContext = contextAccessor.HttpContext;
            _dbContext = dbContext;
        }

        public const string default_scope = "snsapi_userinfo";

        [HttpGet]
        public ActionResult Authorize([FromQuery] string code, [FromQuery] long state, [FromQuery] string jxwechat)
        {
            var props = _dbContext.PropertySettings
                .Where(x => x.ChannelCode.Equals(ChannelCodes.App) && x.ChannelAppId == state).ToList();

            if (props.Count < 1)
            {
                return NotFound("请先配置应用");
            }

            var WechatClientID = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WechatClientID)).Value;
            var WechatClientSecret = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WechatClientSecret)).Value;
            var WechatScope = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WechatScope)).Value;
            var WechatRedirectUri = props
                .FirstOrDefault(x => x.Name.Equals(PropKeyConst.WechatRedirectUri)).Value;

            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(jxwechat))
            {
                var host = _httpContext.Request.Scheme + "://" + _httpContext.Request.Host.ToString();

                var redirect_uri = $"{host}/Wechat/Authorize";

                var queryStrings = $"appid={WechatClientID}&redirect_uri={redirect_uri}&response_type=code&scope={WechatScope}&state={state}#wechat_redirect";

                var redirectTo = string.Empty;

                if (WechatScope.Equals("snsapi_login"))
                {
                    redirectTo = Authorize_url2 + "?" + queryStrings;
                }
                else
                {
                    redirectTo = Authorize_url + "?" + queryStrings;
                }

                return new RedirectResult(redirectTo);
            }

            else
            {
                var OAuthorizeResult = GetAccessToken(WechatClientID, WechatClientSecret, code, WechatScope);

                var redirectTo = WechatRedirectUri + $"?id={state}&app_wechat={HttpUtility.UrlEncode(OAuthorizeResult)}";

                return new RedirectResult(redirectTo);
            }
        }

        private static string GetAccessToken(string client_id, string client_secret, string code, string scope)
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