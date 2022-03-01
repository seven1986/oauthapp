using System;
using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class EmailSignInRequest
    {
        public long AppID { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string VerifyCode { get; set; }
    }
}
