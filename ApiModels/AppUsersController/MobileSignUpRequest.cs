using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class MobileSignUpRequest
    {
        public long AppID { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Pwd { get; set; }

        [Required]
        public string VerifyCode { get; set; }

        public string Avatar { get; set; }
    }
}
