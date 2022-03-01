using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.UserController
{
    public class UserSignUpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [RegularExpression("[a-zA-Z0-9_-]{6,12}")]
        public string Pwd { get; set; }
    }
}
