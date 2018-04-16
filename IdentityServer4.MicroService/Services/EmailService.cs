using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;

namespace IdentityServer4.MicroService.Services
{
    /// <summary>
    /// 发邮件验证码专用
    /// </summary>
    public class EmailService
    {
        readonly ITimeLimitedDataProtector protector;
        // 邮件
        readonly IEmailSender email;

        public EmailService(
            IEmailSender _email, 
            IDataProtectionProvider _provider)
        {
            email = _email;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
        }


        /// <summary>
        /// 验证邮箱收到的验证码是否有效
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool VerifyCode(string str)
        {
            try
            {
                protector.Unprotect(str);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <param name="subject">邮件标题</param>
        /// <param name="templateKey">邮件模板ID</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="expiredIn">验证码有效时间（秒）</param>
        /// <param name="emailAddress">收件地址</param>
        /// <returns></returns>
        public async Task<bool> SendCode(string subject, string templateKey,
            string verifyCode, TimeSpan expiredIn, string emailAddress)
        {
            try
            {
                // 用加密算法生成具有时效性的密文
                var protectedData = protector.Protect(verifyCode, expiredIn);

                var xsmtpapi = JsonConvert.SerializeObject(new
                {
                    to = new string[] { emailAddress },
                    sub = new Dictionary<string, string[]>()
                        {
                            { "%code%", new string[] { protectedData } },
                        }
                });

                await email.SendEmailAsync(subject, templateKey, xsmtpapi);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
