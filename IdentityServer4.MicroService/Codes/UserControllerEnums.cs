using System.ComponentModel;

namespace IdentityServer4.MicroService.Codes
{
    /// <summary>
    /// 用户接口业务代码常量表
    /// </summary>
    public class UserControllerEnums
    {
        /// <summary>
        /// 提交报名 - 业务代码
        /// </summary>
        public enum ApplyFor
        {
            /// <summary>
            /// 手机号已被注册
            /// </summary>
            [Description("手机号已被注册")]
            PhoneNumberExists = 406,

            /// <summary>
            /// 无效的手机验证码
            /// </summary>
            [Description("无效的手机验证码")]
            PhoneNumberVerifyCodeError = 407,
        }

        /// <summary>
        /// 发送手机验证码 - 业务代码
        /// </summary>
        public enum VerifyPhoneNumber
        {
            /// <summary>
            /// 该号码已达24小时发送上限
            /// </summary>
            [Description("该号码已达24小时发送上限")]
            CallLimited = 430,

            /// <summary>
            /// 发送过于频繁,请{0}秒后重试
            /// </summary>
            [Description("发送过于频繁,请{0}秒后重试")]
            TooManyRequests = 429
        }

        /// <summary>
        /// 发送手机验证码 - 业务代码
        /// </summary>
        public enum VerifyEmailAddress
        {
            /// <summary>
            /// 该号码已达24小时发送上限
            /// </summary>
            [Description("该号码已达24小时发送上限")]
            CallLimited = 430,

            /// <summary>
            /// 发送过于频繁,请{0}秒后重试
            /// </summary>
            [Description("发送过于频繁,请{0}秒后重试")]
            TooManyRequests = 429
        }
    }
}
