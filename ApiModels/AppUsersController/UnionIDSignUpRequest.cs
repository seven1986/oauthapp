using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class UnionIDSignUpRequest
    {
        public long AppID { get; set; }

        [Required]
        public string UnionID { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Pwd { get; set; }

        public string Avatar { get; set; }
    }
}
