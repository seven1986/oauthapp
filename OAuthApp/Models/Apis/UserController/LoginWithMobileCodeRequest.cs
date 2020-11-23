using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.UserController
{
   public class LoginWithMobileCodeRequest
    {
        /// <summary>
        /// 国家代码
        /// </summary>
        [Required(ErrorMessage = "请填写国家代码")]
        public string CountryCode { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        [Required(ErrorMessage = "请填写手机号码")]
        [Phone(ErrorMessage = "手机号码格式错误")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// PhoneNumber VerifyCode
        /// </summary>
        [Required(ErrorMessage = "请填写手机验证码")]
        [RegularExpression("[0-9]{4,6}", ErrorMessage = "验证码为格式错误")]
        public string PhoneNumberVerifyCode { get; set; }
    }
}
