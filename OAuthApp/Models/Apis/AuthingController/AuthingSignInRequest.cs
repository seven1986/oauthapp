using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.AuthingController
{
  public class AuthingSignInRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 授权地址
        /// </summary>
        [Required]
        public string ReturnUrl { get; set; }
    }
}
