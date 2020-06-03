using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserGetRequest
    {
        /// <summary>
        /// 用户角色
        /// 可选值：user/partner/developer/administrator
        /// </summary>
        public string role { get; set; }
        
        /// <summary>
        /// 手机号
        /// </summary>
        [Phone(ErrorMessage = "手机号码格式错误")]
        public string phoneNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string email { get; set; }

        /// <summary>
        /// 第三方登录平台名称
        /// </summary>
        public string providerName { get; set; }

        /// <summary>
        /// 第三方登陆平台的UserID
        /// </summary>

        public string providerKey { get; set; }

        /// <summary>
        /// claimType
        /// </summary>
        public string claimType { get; set; }

        /// <summary>
        /// claimValue
        /// </summary>
        public string claimValue { get; set; }
    }
}
