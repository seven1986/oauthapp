using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.AuthingController
{
   public class AuthingAuthinChangePhoneRequest
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
