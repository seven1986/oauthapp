using System;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.AuthingController
{
   public class AuthingPreConsentRequest
    {
        [Required]
        public string ReturnUrl { get; set; }
    }
}
