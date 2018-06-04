using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Host.Models.Views.Manage
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
