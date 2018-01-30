using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace IdentityServer4.MicroService.Services
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

        public ILogger Logger { get;  }

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
}
