using System;

namespace IdentityServer4.MicroService.Attributes
{
   public class EmailConfigAttribute : Attribute
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

        public EmailConfigAttribute() { }

        public EmailConfigAttribute(string _subject)
        {
            subject = _subject;
        }
    }
}
