using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserRegisterRequest
    {
        /// <summary>
        /// NickName
        /// </summary>
        [Required(ErrorMessage = "请填写姓名")]
        [StringLength(50, ErrorMessage = "2-50个字节之内", MinimumLength = 2)]
        public string NickName { get; set; }


        [Required(ErrorMessage = "请填写联系邮箱")]
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        [Required(ErrorMessage = "请选择性别")]
        [StringLength(10, ErrorMessage = "1-10个字节之内", MinimumLength = 1)]
        public string Gender { get; set; }

        /// <summary>
        /// Adress
        /// </summary>
        [Required(ErrorMessage = "请填写联系地址")]
        [StringLength(500, ErrorMessage = "1-500个字节之内", MinimumLength = 1)]
        public string Address { get; set; }

        /// <summary>
        /// Birth
        /// </summary>
        [Required(ErrorMessage = "请填写出生年月")]
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Stature
        /// </summary>
        [Required(ErrorMessage = "请填写身高")]
        [Range(0.1, 99, ErrorMessage = "0.1-99之内")]
        public decimal Stature { get; set; }

        /// <summary>
        /// Weight
        /// </summary>
        [Required(ErrorMessage = "请填写体重")]
        [Range(1, 999, ErrorMessage = "1-999之内")]
        public decimal Weight { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

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

        // 非必填，如果填写了将进行有效性的验证
        public string EmailVerifyCode { get; set; }

        /// <summary>
        /// photos（max for 9）
        /// </summary>
        public List<string> ImageUrl { get; set; }

        /// <summary>
        /// Video（max for 20M）
        /// </summary>
        public string Video { get; set; }

        /// <summary>
        /// Doc（PDF/DOC max for 10M）
        /// </summary>
        public string Doc { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
