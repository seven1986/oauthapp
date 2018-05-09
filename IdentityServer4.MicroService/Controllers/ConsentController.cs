using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.Views.Concent;
using IdentityServer4.MicroService.Tenant;
using System;

namespace IdentityServer4.MicroService.Controllers
{
    public class ConsentController: BasicController
    {
        private readonly ConsentService _consent;

        public ConsentController(
            Lazy<IIdentityServerInteractionService> interaction,
            Lazy<IClientStore> clientStore,
            Lazy<IResourceStore> resourceStore,
            Lazy<ILogger<ConsentController>> logger,
            Lazy<TenantService> _tenantService,
            Lazy<TenantDbContext> _tenantDb)
        {
            _consent = new ConsentService(interaction.Value, clientStore.Value, resourceStore.Value, logger.Value);
            tenantService = _tenantService.Value;
            tenantDb = _tenantDb.Value;
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _consent.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consent.ProcessConsent(model);

            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }

            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }

            if (result.ShowView)
            {
                return View("Index", result.ViewModel);
            }

            return View("Error");
        }
    }
}
