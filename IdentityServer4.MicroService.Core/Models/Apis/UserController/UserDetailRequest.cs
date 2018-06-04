using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserDetailRequest
    {
        /// <summary>
        /// PhoneNumber
        /// </summary>
        [Required(ErrorMessage = "请填写手机号码")]
        [Phone(ErrorMessage = "手机号码格式错误")]
        public string PhoneNumber { get; set; }
    }
}
