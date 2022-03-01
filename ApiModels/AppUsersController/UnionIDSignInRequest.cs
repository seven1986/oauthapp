using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class UnionIDSignInRequest
    {
        public long AppID { get; set; }

        [Required]
        public string UnionID { get; set; }

        [Required]
        public string Platform { get; set; }

    }
}
