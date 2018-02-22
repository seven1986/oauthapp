using System.ComponentModel;

namespace IdentityServer4.MicroService.Enums
{
    /// <summary>
    /// 用户接口业务代码常量表
    /// </summary>
    public enum UserControllerEnum
    {

        #region 提交报名 - 业务代码
        /// <summary>
        /// 手机号已被注册
        /// </summary>
        [Description("手机号已被注册")]
        Register_PhoneNumberExists = 100001,

        /// <summary>
        /// 无效的手机验证码
        /// </summary>
        [Description("无效的手机验证码")]
        Register_PhoneNumberVerifyCodeError = 100002,

        /// <summary>
        /// 手机号已被注册
        /// </summary>
        [Description("手机号已被注册")]
        Register_EmailExists = 100003,

        /// <summary>
        /// 手机号已被注册
        /// </summary>
        [Description("手机号已被注册")]
        Register_EmailVerifyCodeError = 100004,
        #endregion


        #region 发送手机验证码
        /// <summary>
        /// 该号码已达24小时发送上限
        /// </summary>
        [Description("该号码已达24小时发送上限")]
        VerifyPhone_CallLimited = 100005,

        /// <summary>
        /// 发送过于频繁,请{0}秒后重试
        /// </summary>
        [Description("发送过于频繁,请{0}秒后重试")]
        VerifyPhone_TooManyRequests = 100006,
        #endregion


        #region 发送手机验证码
        /// <summary>
        /// 该号码已达24小时发送上限
        /// </summary>
        [Description("该号码已达24小时发送上限")]
        VerifyEmail_CallLimited = 100007,

        /// <summary>
        /// 发送过于频繁,请{0}秒后重试
        /// </summary>
        [Description("发送过于频繁,请{0}秒后重试")]
        VerifyEmail_TooManyRequests = 100008
        #endregion
    }
}
