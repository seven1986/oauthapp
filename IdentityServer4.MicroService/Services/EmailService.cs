using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static IdentityServer4.MicroService.AppDefaultData;

namespace IdentityServer4.MicroService.Services
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    public class EmailService
    {
        // 邮件服务
        readonly IEmailSender email;

        public EmailService(
            IEmailSender _email)
        {
            email = _email;
        }

        ///// <summary>
        ///// 发送邮箱验证码
        ///// </summary>
        ///// <param name="subject">邮件标题</param>
        ///// <param name="templateKey">邮件模板ID</param>
        ///// <param name="emailAddress">收件地址</param>
        ///// <param name="vars"></param>
        ///// <returns></returns>
        //public async Task<bool> SendCode(
        //    string subject, 
        //    string templateKey,
        //    string emailAddress,
        //    Dictionary<string,string[]> vars)
        //{
        //    try
        //    {
        //        var xsmtpapi = JsonConvert.SerializeObject(new
        //        {
        //            to = new string[] { emailAddress },
        //            sub = vars
        //        });

        //        await email.SendEmailAsync(subject, templateKey, xsmtpapi);

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="template">邮件模板ID</param>
        /// <param name="emailAddress">收件地址</param>
        /// <param name="vars">变量集合</param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(
            SendCloudMailTemplates template, 
            string emailAddress, 
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
                to = new string[] { emailAddress },
                sub
            });

            try
            {
                await email.SendEmailAsync(
                    TemplateEntity.subject, 
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
}
