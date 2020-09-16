using System;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace OAuthApp.Services
{
    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(string smsVars, string phone, string templateId);

        Task<bool> SendSmsWithRetryAsync(string smsVars, string phone, string templateId, int retryCount = 3);
    }

    public class SmsSenderOptions
    {
        public string apiUser { get; set; }

        public string apiKey { get; set; }
    }

    public class SmsSender : ISmsSender
    {
        public SmsSenderOptions Options { get; }

        public ILogger Logger { get; }

        public SmsSender(IOptions<SmsSenderOptions> optionsAccessor, ILoggerFactory logger)
        {
            Options = optionsAccessor.Value;

            Logger = logger.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// 发送短信
        /// （服务商：https://sendcloud.sohu.com）
        /// </summary>
        /// <param name="smsVars">模板变量</param>
        /// <param name="phone">电话号码</param>
        /// <param name="templateId">模板ID(9812)</param>
        /// <returns>true成功false失败</returns>
        public async Task<bool> SendSmsAsync(string smsVars, string phone, string templateId)
        {
            var Timestamp = await _ServerTimestamp();

            if (string.IsNullOrWhiteSpace(Timestamp)) { return false; }

            var data = new SortedDictionary<string, string>()
            {
                { "smsUser",Options.apiUser},
                { "templateId",templateId},
                { "msgType","0"},
                { "phone",phone},
                { "vars",smsVars},
                { "timestamp",Timestamp},
            };

            var signatureString = $"{Options.apiKey}&" + 
                string.Join("&", data.Select(x => $"{x.Key}={x.Value}").ToArray()) + 
                $"&{Options.apiKey}";

            data.Add("signature", _MD5String(signatureString));

            using (var c = new HttpClient())
            {
                try
                {
                    var response = await c.PostAsync("http://www.sendcloud.net/smsapi/send",
                        new FormUrlEncodedContent(data));

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        var responseJson = JObject.Parse(responseString);

                        var statusCode = responseJson["statusCode"].Value<string>();

                        if (statusCode.Equals("200"))
                        {
                            return true;
                        }
                        else
                        {
                            Logger.LogError(responseString);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Send Sms Error!Message :{0} ", ex.Message);
                }
            }

            return false;
        }

        /// <summary>
        /// sendcloud服务器的时间戳
        /// </summary>
        /// <returns></returns>
        async Task<string> _ServerTimestamp()
        {
            var result = string.Empty;

            using (var hc = new HttpClient())
            {
                var requestUri = "http://www.sendcloud.net/smsapi/timestamp/get";

                var response = await hc.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = response.Content.ReadAsStringAsync().Result;

                    try
                    {
                        result = JObject.Parse(responseString)["info"]["timestamp"].Value<string>();
                    }
                    catch
                    {

                    }
                }
            }
            return result;
        }

        string _MD5String(string txt)
        {
            var txtBytes = Encoding.UTF8.GetBytes(txt);

            var output = MD5.Create().ComputeHash(txtBytes);

            var result = BitConverter.ToString(output).Replace("-", "");

            return result;
        }

        public async Task<bool> SendSmsWithRetryAsync(string smsVars, string phone, string templateId, int retryCount = 3)
        {
            var _retryCount = 0;

            while (!await SendSmsAsync(smsVars, phone, templateId))
            {
                if (_retryCount > retryCount)
                {
                    return false;
                }

                _retryCount++;

                Thread.Sleep(10);
            }

            return true;
        }
    }
}
