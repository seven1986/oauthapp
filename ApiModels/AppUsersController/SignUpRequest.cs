using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class SignUpRequest
    {
        public long AppID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string NickName { get; set; }

        [Required]
        public string Platform { get; set; }

        [Required]
        public string Pwd { get; set; }

        public string Avatar { get; set; }
    }
       
}
