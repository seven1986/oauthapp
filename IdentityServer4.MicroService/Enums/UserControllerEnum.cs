using System.ComponentModel;

namespace IdentityServer4.MicroService.Enums
{
    /// <summary>
    /// 用户接口业务代码常量表
    /// </summary>
    public class UserControllerEnum
    {
        /// <summary>
        /// 提交报名 - 业务代码
        /// </summary>
        public enum Register
        {
            /// <summary>
            /// 手机号已被注册
            /// </summary>
            [Description("手机号已被注册")]
            PhoneNumberExists = 100001,

            /// <summary>
            /// 无效的手机验证码
            /// </summary>
            [Description("无效的手机验证码")]
            PhoneNumberVerifyCodeError = 100002,

            /// <summary>
            /// 手机号已被注册
            /// </summary>
            [Description("手机号已被注册")]
            EmailExists = 100003,

            /// <summary>
            /// 手机号已被注册
            /// </summary>
            [Description("手机号已被注册")]
            EmailVerifyCodeError = 100004,
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
            CallLimited = 100005,

            /// <summary>
            /// 发送过于频繁,请{0}秒后重试
            /// </summary>
            [Description("发送过于频繁,请{0}秒后重试")]
            TooManyRequests = 100006
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
            CallLimited = 100007,

            /// <summary>
            /// 发送过于频繁,请{0}秒后重试
            /// </summary>
            [Description("发送过于频繁,请{0}秒后重试")]
            TooManyRequests = 100008
        }
    }
}
