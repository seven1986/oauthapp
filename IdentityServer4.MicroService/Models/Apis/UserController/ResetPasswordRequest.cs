using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
   public class ResetPasswordRequest
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        [Required]
        public string CountryCode { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// PhoneNumber VerifyCode
        /// </summary>
        [Required]
        public string PhoneNumberVerifyCode { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
