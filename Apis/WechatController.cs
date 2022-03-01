using OAuthApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace OAuthApp.Apis
{
    [SwaggerTag("微信")]
    public class WechatController:BaseController
    {
        #region services
        private readonly AppDbContext _context;
        #endregion

        #region construct
        public WechatController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        const string access_token_Uri = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";

        const string subscribe_send_Uri = "https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={0}";

        const string jscode2session_Uri = "https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";

        const string userinfo_Uri = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";

        const string subscribe_Uri = "https://api.weixin.qq.com/cgi-bin/message/template/subscribe?access_token={0}";

        const string jsapi_ticket_Uri = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi";

        const string generate_urllink_Uri = "https://api.weixin.qq.com/wxa/generate_urllink?access_token={0}";

        const string wxacodeget_Uri = "https://api.weixin.qq.com/wxa/getwxacode?access_token={0}";


        readonly ConcurrentDictionary<long, KeyValuePair<string, long>> AccessTokenStore =
            new ConcurrentDictionary<long, KeyValuePair<string, long>>();

        readonly ConcurrentDictionary<long, KeyValuePair<string, long>> MiniPAccessTokenStore =
            new ConcurrentDictionary<long, KeyValuePair<string, long>>();

        readonly ConcurrentDictionary<long, KeyValuePair<string, long>> JsAPITicketStore =
                new ConcurrentDictionary<long, KeyValuePair<string, long>>();

        private string GetToken(long appId, bool IsMiniP)
        {
            var tokenStore = IsMiniP ? MiniPAccessTokenStore : AccessTokenStore;

            if (!tokenStore.ContainsKey(appId))
            {
                var tokenResult = RequestToken(appId, IsMiniP);

                if (tokenResult != null)
                {
                    tokenStore[appId] = new KeyValuePair<string, long>(tokenResult.Item1,
                            DateTime.UtcNow.AddHours(8).AddSeconds(tokenResult.Item2).Ticks);
                }

                return tokenResult.Item1;
            }
            else
            {
                var tokenData = tokenStore[appId];

                if (new DateTime(tokenData.Value) > DateTime.UtcNow.AddHours(8))
                {
                    return tokenData.Key;
                }

                else
                {
                    var tokenResult = RequestToken(appId, IsMiniP);

                    if (tokenResult != null)
                    {
                        tokenStore[appId] = new KeyValuePair<string, long>(tokenResult.Item1,
                            DateTime.UtcNow.AddHours(8).AddSeconds(tokenResult.Item2).Ticks);
                    }

                    return tokenResult.Item1;
                }
            }
        }

        private Tuple<string, long> RequestToken(long appId, bool IsMiniP)
        {
            var config = IsMiniP ? WechatMiniPConfig(appId) : WechatConfig(appId);

            if (string.IsNullOrWhiteSpace(config.Item1))
            {
                return null;
            }

            var tokenResult = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(access_token_Uri, config.Item1, config.Item2);

                tokenResult = hc.GetStringAsync(url).Result;
            }

            var tokenResultJson = JsonConvert.DeserializeObject<JObject>(tokenResult);

            if (tokenResultJson != null && tokenResultJson["access_token"] != null)
            {
                var token = tokenResultJson["access_token"].Value<string>();

                var expires_in = tokenResultJson["expires_in"].Value<long>();

                return new Tuple<string, long>(token, expires_in);
            }

            return null;
        }

        private Tuple<string, long> RequestJSApiToken(long appId)
        {
            var tokenResult = string.Empty;

            using (var hc = new HttpClient())
            {
                var accessToken = GetToken(appId, false);

                var url = string.Format(jsapi_ticket_Uri, accessToken);

                tokenResult = hc.GetStringAsync(url).Result;
            }

            var tokenResultJson = JsonConvert.DeserializeObject<JObject>(tokenResult);

            if (tokenResultJson != null && tokenResultJson["ticket"] != null)
            {
                var token = tokenResultJson["ticket"].Value<string>();

                var expires_in = tokenResultJson["expires_in"].Value<long>();

                return new Tuple<string, long>(token, expires_in);
            }

            return null;
        }

        private string GetJSApiToken(long appId)
        {
            if (!JsAPITicketStore.ContainsKey(appId))
            {
                var tokenResult = RequestJSApiToken(appId);

                if (tokenResult != null)
                {
                    JsAPITicketStore[appId] = new KeyValuePair<string, long>(tokenResult.Item1,
                            DateTime.UtcNow.AddHours(8).AddSeconds(tokenResult.Item2).Ticks);
                }

                return tokenResult.Item1;
            }
            else
            {
                var tokenData = JsAPITicketStore[appId];

                if (new DateTime(tokenData.Value) > DateTime.UtcNow.AddHours(8))
                {
                    return tokenData.Key;
                }

                else
                {
                    var tokenResult = RequestJSApiToken(appId);

                    if (tokenResult != null)
                    {
                        JsAPITicketStore[appId] = new KeyValuePair<string, long>(tokenResult.Item1,
                            DateTime.UtcNow.AddHours(8).AddSeconds(tokenResult.Item2).Ticks);
                    }

                    return tokenResult.Item1;
                }
            }
        }

        private (string, string, string) WechatMiniPConfig(long appId)
        {
            var props = _context.PropertySettings.Where(x =>
           x.ChannelCode == ChannelCodes.App && x.ChannelAppId == appId).ToList();

            var WechatMiniPClientID = props.Where(x => x.Name.Equals(PropKeyConst.WechatMiniPClientID))
                .Select(x => x.Value).FirstOrDefault();

            var WechatMiniPClientSecret = props.Where(x => x.Name.Equals(PropKeyConst.WechatMiniPClientSecret))
                .Select(x => x.Value).FirstOrDefault();

            var WechatJSApiList = props.Where(x => x.Name.Equals(PropKeyConst.WechatJSApiList))
                .Select(x => x.Value).FirstOrDefault();

            return (WechatMiniPClientID, WechatMiniPClientSecret, WechatJSApiList);
        }

        private (string, string, string) WechatConfig(long appId)
        {
            var props = _context.PropertySettings.Where(x =>
            x.ChannelCode == ChannelCodes.App && x.ChannelAppId == appId).ToList();

            var WechatClientID = props.Where(x => x.Name.Equals(PropKeyConst.WechatClientID))
                .Select(x => x.Value).FirstOrDefault();

            var WechatClientSecret = props.Where(x => x.Name.Equals(PropKeyConst.WechatClientSecret))
                .Select(x => x.Value).FirstOrDefault();

            var WechatJSApiList = props.Where(x => x.Name.Equals(PropKeyConst.WechatJSApiList))
                .Select(x => x.Value).FirstOrDefault();

          
            return (WechatClientID, WechatClientSecret, WechatJSApiList);
        }

        #region 发送订阅消息
        /// <summary>
        /// 发送订阅消息
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="data">开发参考：https://dwz.cn/bohXaCnp </param>
        /// <returns></returns>
        [HttpPost("SubscribeSend")]
        [SwaggerOperation(OperationId = "WechatSubscribeSend")]
        [AllowAnonymous]
        public IActionResult SubscribeSend([FromQuery] long appID, [FromBody] JObject data)
        {
            var token = GetToken(appID, true);

            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(subscribe_send_Uri, token);

                var PostResult = hc.PostAsync(url,
                    new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

                if (PostResult.IsSuccessStatusCode)
                {
                    result = PostResult.Content.ReadAsStringAsync().Result;
                }

                else
                {
                    result = PostResult.StatusCode + PostResult.ReasonPhrase;
                }
            }

            return OK(result);
        } 
        #endregion

        #region 获取用户标识
        /// <summary>
        /// 获取用户标识
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="js_code">开发参考：https://dwz.cn/icNajFh7 </param>
        /// <returns></returns>
        [HttpGet("JSCode2Session")]
        [SwaggerOperation(OperationId = "WechatJSCode2Session", Description = "开发参考：https://dwz.cn/icNajFh7")]
        [AllowAnonymous]
        public IActionResult JSCode2Session([FromQuery] long appID, [FromQuery] string js_code)
        {
            var minipConfig = WechatMiniPConfig(appID);

            if (string.IsNullOrWhiteSpace(minipConfig.Item1))
            {
                return Error("");
            }

            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(jscode2session_Uri, minipConfig.Item1, minipConfig.Item2, js_code);

                var PostResult = hc.GetAsync(url).Result;

                if (PostResult.IsSuccessStatusCode)
                {
                    result = PostResult.Content.ReadAsStringAsync().Result;
                }

                else
                {
                    result = PostResult.StatusCode + PostResult.ReasonPhrase;
                }
            }

            return OK(result);
        } 
        #endregion

        #region 解密数据
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="encryptedData">加密的数据</param>
        /// <param name="iv"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        [HttpGet("Decrypt")]
        [SwaggerOperation(OperationId = "WechatDecrypt")]
        [AllowAnonymous]
        public IActionResult Decrypt([FromQuery] string encryptedData, [FromQuery] string iv, [FromQuery] string sessionKey)
        {
            var aes = new AesCryptoServiceProvider()
            {
                Mode = CipherMode.CBC,
                BlockSize = 128,
                Padding = PaddingMode.PKCS7,
                IV = Convert.FromBase64String(iv),
                Key = Convert.FromBase64String(sessionKey)
            };

            var transform = aes.CreateDecryptor();

            var byte_encryptedData = Convert.FromBase64String(encryptedData);

            var decryptedData = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);

            string result = Encoding.UTF8.GetString(decryptedData);

            return OK(result);
        } 
        #endregion

        #region 获取用户基本信息(UnionID机制)
        /// <summary>
        /// 获取用户基本信息(UnionID机制)
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [HttpGet("UserInfo")]
        [SwaggerOperation(OperationId = "WechatUserInfo")]
        [AllowAnonymous]
        public IActionResult UserInfo([FromQuery] long appID, [FromQuery] string openid)
        {
            var token = GetToken(appID, false);

            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(userinfo_Uri, token, openid);

                var GetResult = hc.GetAsync(url).Result;

                if (GetResult.IsSuccessStatusCode)
                {
                    result = GetResult.Content.ReadAsStringAsync().Result;
                }

                else
                {
                    result = GetResult.StatusCode + GetResult.ReasonPhrase;
                }
            }

            return OK(result);
        } 
        #endregion

        #region 发送一次性订阅消息
        /// <summary>
        /// 发送一次性订阅消息
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="data">开发参考：https://dwz.cn/IXptek5n </param>
        /// <returns></returns>
        [HttpPost("SubscribeMSG")]
        [SwaggerOperation(OperationId = "WechatSubscribeMSG")]
        [AllowAnonymous]
        public IActionResult SubscribeMSG([FromQuery] long appID, [FromBody] JObject data)
        {
            var token = GetToken(appID, false);

            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(subscribe_Uri, token);

                var PostResult = hc.PostAsync(url,
                    new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

                if (PostResult.IsSuccessStatusCode)
                {
                    result = PostResult.Content.ReadAsStringAsync().Result;
                }

                else
                {
                    result = PostResult.StatusCode + PostResult.ReasonPhrase;
                }
            }

            return OK(result);
        } 
        #endregion

        #region JS SDK Config
        /// <summary>
        /// JS SDK Config
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet("JSConfig")]
        [SwaggerOperation(OperationId = "WechatJSConfig")]
        [AllowAnonymous]
        public IActionResult JSConfig([FromQuery] long appID, [FromQuery] string url)
        {
            var noncestr = Guid.NewGuid().ToString("n");
            var timestamp = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();

            var jsapi_ticket = GetJSApiToken(appID);

            if (string.IsNullOrWhiteSpace(url))
            {
                url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}{Request.QueryString}";
            }

            var sortedDict = new SortedDictionary<string, string>() {
                { "noncestr",noncestr},
                { "jsapi_ticket",jsapi_ticket},
                { "timestamp",timestamp},
                { "url",url},
            };

            var string1 = string.Join("&", sortedDict.Select(x => $"{x.Key}={x.Value}"));

            var signature = SHA1Encrypt(string1, true, true);

            var config = WechatConfig(appID);

            var appId = config.Item1;

            var jsApiList = new List<string>();

            if (!string.IsNullOrWhiteSpace(config.Item3))
            {
                jsApiList = config.Item3.Split(new string[1] { "," },
                    StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return OK(new
            {
                noncestr,
                timestamp,
                signature,
                appId,
                jsApiList
            });
        } 
        #endregion

        #region 用SHA1加密字符串
        /// <summary>
        /// 用SHA1加密字符串
        /// </summary>
        /// <param name="source">要扩展的对象</param>
        /// <param name="isReplace">是否替换掉加密后的字符串中的"-"字符</param>
        /// <param name="isToLower">是否把加密后的字符串转小写</param>
        /// <returns></returns>
        private static string SHA1Encrypt(string source, bool isReplace = true, bool isToLower = false)
        {
            var sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(source));
            string shaStr = BitConverter.ToString(hash);
            if (isReplace)
            {
                shaStr = shaStr.Replace("-", "");
            }
            if (isToLower)
            {
                shaStr = shaStr.ToLower();
            }
            return shaStr;
        }
        #endregion

        #region 获取小程序 URL Link
        /// <summary>
        /// 获取小程序 URL Link，适用于短信、邮件、网页、微信内等拉起小程序的业务场景
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="data">开发参考：https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/url-link/urllink.generate.html </param>
        /// <returns></returns>
        [HttpPost("UrlLinkGenerate")]
        [SwaggerOperation(OperationId = "WechatUrlLinkGenerate")]
        [AllowAnonymous]
        public IActionResult UrlLinkGenerate([FromQuery] long appID, [FromBody] JObject data)
        {
            var token = GetToken(appID, true);

            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var url = string.Format(generate_urllink_Uri, token);

                var PostResult = hc.PostAsync(url,
                    new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

                if (PostResult.IsSuccessStatusCode)
                {
                    result = PostResult.Content.ReadAsStringAsync().Result;
                }

                else
                {
                    result = PostResult.StatusCode + PostResult.ReasonPhrase;
                }
            }

            return OK(result);
        }
        #endregion

        #region 获取小程序码
        /// <summary>
        /// 获取小程序码
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="data">开发参考：https://developers.weixin.qq.com/miniprogram/dev/api-backend/open-api/qr-code/wxacode.get.html </param>
        /// <returns></returns>
        [HttpPost("WXACodeGet")]
        [SwaggerOperation(OperationId = "WechatWXACodeGet")]
        [AllowAnonymous]
        public IActionResult WXACodeGet([FromQuery] long appID, [FromBody] JObject data)
        {
            var token = GetToken(appID, true);

            if(string.IsNullOrWhiteSpace(token))
            {
                return Error("未配置应用");
            }

            using var hc = new HttpClient();

            var url = string.Format(wxacodeget_Uri, token);

            var PostResult = hc.PostAsync(url,
                new StringContent(data.ToString(), Encoding.UTF8, "application/json")).Result;

            if (PostResult.IsSuccessStatusCode)
            {
                var result = PostResult.Content.ReadAsStreamAsync().Result;

                return new FileStreamResult(result, "image/jpeg");
            }

            else
            {
                var errData = Encoding.UTF8.GetBytes(PostResult.StatusCode.ToString() + PostResult.ReasonPhrase);

                using var ms = new MemoryStream(errData);

                return new FileStreamResult(ms, "text/plain");
            }
        }
        #endregion
    }
}
