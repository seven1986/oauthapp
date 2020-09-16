using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static OAuthApp.AppDefaultData;

namespace OAuthApp.Services
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string subject, string templateInvokeName, string xsmtpapi, string from = "", string fromName = "");
    }

    public class EmailSenderOptions
    {
        public string apiUser { get; set; }

        public string apiKey { get; set; }

        public string fromEmail { get; set; }

        public string fromName { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        public EmailSenderOptions Options { get; private set; }

        public ILogger Logger { get; }

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor, ILoggerFactory logger)
        {
            Options = optionsAccessor.Value;
            Logger = logger.CreateLogger(GetType().FullName);
        }

        public async Task<bool> SendEmailAsync(string subject, string templateInvokeName, string xsmtpapi, string from, string fromName)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                from = Options.fromEmail;
            }

            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = Options.fromName;
            }

            var paramList = new SortedDictionary<string, string>()
                {
                    { "apiUser", Options.apiUser },
                    { "apiKey", Options.apiKey},
                    { "from", from},
                    { "fromName",fromName},
                    { "xsmtpapi", xsmtpapi},
                    { "subject", subject},
                    { "templateInvokeName", templateInvokeName},
                };

            var postContent = new FormUrlEncodedContent(paramList);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("http://api.sendcloud.net/apiv2/mail/sendtemplate", postContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        try
                        {
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
                        catch
                        {

                        }
                    }
                }

                catch (Exception e)
                {
                    Logger.LogError("Send Email Error!Message :{0} ", e.Message);
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    public class EmailService
    {
        // 邮件服务
        readonly IEmailSender email;

        public EmailService(IEmailSender _email)
        {
            email = _email;
        }

      

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="templateKey">邮件模板ID</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="emailAddress">收件地址</param>
        /// <param name="vars">变量集合</param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(
            string templateKey,
            string subject,
            string[] emailAddress,
            Dictionary<string, string[]> vars)
        {
            var sub = new Dictionary<string, string[]>();

            foreach (var kv in vars)
            {
                if (!sub.ContainsKey(kv.Key))
                {
                    sub.Add(kv.Key, kv.Value);
                }
            }

            var xsmtpapi = JsonConvert.SerializeObject(new
            {
                to = emailAddress,
                sub
            });

            try
            {
                await email.SendEmailAsync(
                    subject,
                    templateKey,
                    xsmtpapi);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
