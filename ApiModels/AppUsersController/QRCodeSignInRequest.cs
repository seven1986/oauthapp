using System.ComponentModel.DataAnnotations;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class QRCodeSignInRequest
    {
        public long AppID { get; set; }

        [Required]
        public string SignInKey { get; set; }
    }
}
