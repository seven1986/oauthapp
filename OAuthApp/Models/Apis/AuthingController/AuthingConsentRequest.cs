using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ConsentController
{
   public class AuthingConsentRequest
    {
        /// <summary>
        /// 权限
        /// </summary>
        [Required]
        public List<string> Scopes { get; set; }

        /// <summary>
        /// 持久
        /// </summary>
        public bool Remember { get; set; } = true;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        [Required]
        public string ReturnUrl { get; set; }
    }
}
