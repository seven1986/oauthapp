using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserGetRequest
    {
        /// <summary>
        /// 用户角色标识，多个角色用","链接
        /// </summary>
        [RegularExpression("^[0-9,]+$", ErrorMessage = "用户角色标识格式错误")]
        public string roles { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Phone(ErrorMessage = "手机号码格式错误")]
        public string phoneNumber { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string email { get; set; }
    }
}
