namespace OAuthApp.CacheKeys
{
    internal class UserControllerKeys
    {
        #region 电话号码
        /// <summary>
        /// （缓存KEY）
        /// 24小时内发送短信验证码的次数计数
        /// Key的格式：{手机号}
        /// Value的格式： 次数，调用SDK的increase方法即可，每次加1
        /// </summary>
        public const string Limit_24Hour_Verify_Phone = "PhoneVerifyCode-Limit24Hour:";

        /// <summary>
        /// 24小时内发送短信验证码的上限
        /// </summary>
        public const int Limit_24Hour_Verify_MAX_Phone = 999;

        /// <summary>
        /// （缓存KEY）
        /// Key的格式：{手机号}
        /// Value的格式：当前时间的tick            
        /// </summary>
        public const string LastTime_SendCode_Phone = "PhoneVerifyCode-LastSend:";

        /// <summary>
        /// 每次发送验证码的最小间隔时长（秒）
        /// </summary>
        public const int MinimumTime_SendCode_Phone = 60;

        /// <summary>
        /// （缓存KEY）
        /// Key的格式：{手机号} + ":" + {验证码}
        /// Value的格式：任意
        /// </summary>
        public const string VerifyCode_Phone = "PhoneVerifyCode:";

        /// <summary>
        /// 发送手机验证码后，该验证码在服务器的有效时长（秒）
        /// </summary>
        public const int VerifyCode_Expire_Phone = 300;
        #endregion

        #region 邮箱地址
        /// <summary>
        /// （缓存KEY）
        /// 24小时内发送邮件验证码的次数计数
        /// Key的格式：{手机号}
        /// Value的格式： 次数，调用SDK的increase方法即可，每次加1
        /// </summary>
        public const string Limit_24Hour_Verify_Email = "EmailVerifyCode-Limit24Hour:";

        /// <summary>
        /// 24小时内发送邮件验证码的上限
        /// </summary>
        public const int Limit_24Hour_Verify_MAX_Email = 999999;

        /// <summary>
        /// （缓存KEY）
        /// Key的格式：{邮箱地址}
        /// Value的格式：当前时间的tick            
        /// </summary>
        public const string LastTime_SendCode_Email = "EmailVerifyCode-LastSend:";

        /// <summary>
        /// 每次发送验证码的最小间隔时长（秒）
        /// </summary>
        public const int MinimumTime_SendCode_Email = 60;

        /// <summary>
        /// （缓存KEY）
        /// Key的格式：{邮箱地址} + ":" + {验证码}
        /// Value的格式：任意
        /// </summary>
        public const string VerifyCode_Email = "EmailVerifyCode:";

        /// <summary>
        /// 发送邮箱验证码后，该验证码在服务器的有效时长（秒）
        /// </summary>
        public const int VerifyCode_Expire_Email = 1800;
        #endregion
    }
}
