using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class MobileSignInRequest
    {
        public long AppID { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string VerifyCode { get; set; }
    }
}
