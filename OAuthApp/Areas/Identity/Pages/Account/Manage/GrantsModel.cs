using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthApp.Areas.Identity.Pages.Account.Manage
{
    [AllowAnonymous]
    public class GrantsModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clients;
        private readonly IResourceStore _resources;
        private readonly IEventService _events;

        public GrantsModel(IIdentityServerInteractionService interaction,
            IClientStore clients,
            IResourceStore resources,
            IEventService events)
        {
            _interaction = interaction;
            _clients = clients;
            _resources = resources;
            _events = events;
        }


        public IActionResult OnGetAsync()
        {
           // data = await BuildViewModelAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]string clientId)
        {
            await _interaction.RevokeUserConsentAsync(clientId);

            await _events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId));

            return RedirectToPage();
        }

        //private async Task<GrantsViewModel> BuildViewModelAsync()
        //{
        //    var grants = await _interaction.GetAllUserGrantsAsync();

        //    var list = new List<GrantViewModel>();
        //    foreach (var grant in grants)
        //    {
        //        var client = await _clients.FindClientByIdAsync(grant.ClientId);
        //        if (client != null)
        //        {
        //            var resources = await _resources.FindResourcesByScopeAsync(grant.Scopes);

        //            var item = new GrantViewModel()
        //            {
        //                ClientId = client.ClientId,
        //                ClientName = client.ClientName ?? client.ClientId,
        //                ClientLogoUrl = client.LogoUri,
        //                ClientUrl = client.ClientUri,
        //                Description = grant.Description,
        //                Created = grant.CreationTime,
        //                Expires = grant.Expiration,
        //                IdentityGrantNames = resources.IdentityResources.Select(x => x.DisplayName ?? x.Name).ToArray(),
        //                ApiGrantNames = resources.ApiScopes.Select(x => x.DisplayName ?? x.Name).ToArray()
        //            };

        //            list.Add(item);
        //        }
        //    }

        //    return new GrantsViewModel
        //    {
        //        Grants = list
        //    };
        //}
    }
}
