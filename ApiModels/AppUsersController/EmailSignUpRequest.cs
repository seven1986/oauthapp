using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class EmailSignUpRequest
    {
        public long AppID { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Pwd { get; set; }

        [Required]
        public string VerifyCode { get; set; }

        public string Avatar { get; set; }
    }
}
