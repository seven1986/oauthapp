using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.AzureJobs.Services
{
    public class EmailService
    {
        protected string sendCloudApiUser { get; set; }
        protected string sendCloudApiKey { get; set; }
        protected string sendCloudFromEmail { get; set; }
        protected string sendCloudfromName { get; set; }

        public EmailService()
        {
            sendCloudApiUser = ConfigurationManager.AppSettings["sendCloudApiUser"];
            sendCloudApiKey = ConfigurationManager.AppSettings["sendCloudApiKey"];
            sendCloudFromEmail = ConfigurationManager.AppSettings["sendCloudFromEmail"];
            sendCloudfromName = ConfigurationManager.AppSettings["sendCloudfromName"];
        }

        private async Task<bool> Send(string subject, string templateInvokeName, string xsmtpapi, string from = "", string fromName = "")
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                from = sendCloudFromEmail;
            }

            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = sendCloudfromName;
            }

            var postContent = new NameValueCollection()
                {
                    { "apiUser", sendCloudApiUser },
                    { "apiKey", sendCloudApiKey},
                    { "from", from},
                    { "fromName",fromName},
                    { "xsmtpapi", xsmtpapi},
                    { "subject", subject},
                    { "templateInvokeName", templateInvokeName},
                };

            using (var client = new WebClient())
            {
                try
                {
                    var response = await client.UploadValuesTaskAsync(new Uri("http://api.sendcloud.net/apiv2/mail/sendtemplate"),
                        postContent);

                    if (response != null && response.Length > 0)
                    {
                        var responseString = Encoding.UTF8.GetString(response);

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
                                throw new Exception(responseString);
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }

                catch (Exception e)
                {
                    throw e;
                }
            }

            return false;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="template">邮件模板ID</param>
        /// <param name="emailAddress">收件地址</param>
        /// <param name="vars">变量集合</param>
        /// <returns></returns>
        public async Task<bool> SendEmail(
            SendCloudMailTemplates template,
            string[] emailAddress,
            Dictionary<string, string[]> vars)
        {
            var templateKey = Enum.GetName(typeof(SendCloudMailTemplates), template);

            var TemplateEntity = template.GetType().GetField(templateKey).GetCustomAttribute<SendCloudTemplateAttribute>();

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
                await Send(TemplateEntity.subject,
                     templateKey,
                     xsmtpapi);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class SendCloudTemplateAttribute : Attribute
    {
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string subject { get; set; }

        ///// <summary>
        ///// 变量集合,多个变量用英文逗号链接
        ///// 格式：%name%
        ///// 参考文档：http://www.sendcloud.net/doc/guide/rule/#x-smtpapi
        ///// </summary>
        //public string vars { get; set; }

        public SendCloudTemplateAttribute() { }

        public SendCloudTemplateAttribute(string _subject)
        {
            subject = _subject;
        }
    }

    /// <summary>
    /// 邮件发送模板
    /// 需要对接sendcloud平台
    /// https://sendcloud.sohu.com
    /// </summary>
    public enum SendCloudMailTemplates
    {
        #region 用户注册 - 邮箱验证
        /// <summary>
        /// 用户注册 - 邮箱验证
        /// 变量：%code%
        /// </summary>
        [SendCloudTemplate("邮箱验证")]
        verify_email,
        #endregion

        #region 订阅微服务 - 邮箱验证
        /// <summary>
        /// 订阅微服务 - 邮箱验证
        /// 变量：
        /// %SubscritionUrl%
        /// %DelSubscritionUrl%
        /// %apiId%
        /// %npmjQuery%
        /// %npmAngular2%
        /// %serviceName%
        /// </summary>
        [SendCloudTemplate("验证邮箱")]
        verify_apiresource_subscription,
        #endregion

        #region 忘记密码
        /// <summary>
        /// 忘记密码
        /// 变量：%callbackUrl%
        /// </summary>
        [SendCloudTemplate("reset password")]
        reset_password,
        #endregion

        #region 用户登录 - 邮箱安全码
        /// <summary>
        /// 用户登录 - 邮箱安全码
        /// 变量：%code%
        /// </summary>
        [SendCloudTemplate("登录验证码")]
        security_code,
        #endregion

        #region 用户注册 - 激活邮箱
        /// <summary>
        /// 用户注册 - 激活邮箱
        /// 变量：%name%,%url%
        /// </summary>
        [SendCloudTemplate("%name%请激活您的邮箱")]
        test_template_active,
        #endregion

        #region 微服务发布
        /// <summary>
        /// 微服务发布
        /// 变量：%apiId%
        /// </summary>
        [SendCloudTemplate("服务更新通知")]
        apiresource_published_notify,
        #endregion
    }
}
