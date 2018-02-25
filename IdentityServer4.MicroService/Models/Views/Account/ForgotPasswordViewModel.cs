using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Views.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
