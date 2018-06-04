using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer4.MicroService.Host.Models.Views.Manage
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
