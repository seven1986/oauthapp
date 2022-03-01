using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class SignInRequest
    {
        public long AppID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Pwd { get; set; }
    }
}
