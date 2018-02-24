using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserVerifyEmailRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "请填写联系邮箱")]
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }
    }
}
