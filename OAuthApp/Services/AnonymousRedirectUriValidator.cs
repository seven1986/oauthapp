using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.Services
{
    public class AnonymousRedirectUriValidator : IRedirectUriValidator
    {
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            var CheckPostLogoutRedirectUri = string.Empty;

            if (client.Properties.ContainsKey(PropertyKeys.LogoutRedirectUri))
            {
                CheckPostLogoutRedirectUri = client.Properties[PropertyKeys.LogoutRedirectUri];
            }

            if ("false".Equals(CheckPostLogoutRedirectUri.ToLower()))
            {
                return Task.FromResult(true);
            }

            else if (StringCollectionContainsString(AppConstant.WhiteList_Clients, client.ClientId))
            {
                return Task.FromResult(true);
            }

            else if (StringCollectionContainsString(AppConstant.WhiteList_RedirectUris, requestedUri))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            var CheckRedirectUri = string.Empty;
            
            if (client.Properties.ContainsKey(PropertyKeys.RedirectUri))
            {
                CheckRedirectUri = client.Properties[PropertyKeys.RedirectUri];
            }

            if ("false".Equals(CheckRedirectUri.ToLower()))
            {
                return Task.FromResult(true);
            }

            else if (StringCollectionContainsString(AppConstant.WhiteList_Clients, client.ClientId))
            {
                return Task.FromResult(true);
            }

            else if (StringCollectionContainsString(AppConstant.WhiteList_RedirectUris, requestedUri))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(StringCollectionContainsString(client.RedirectUris, requestedUri));
        }

        protected bool StringCollectionContainsString(IEnumerable<string> uris, string requestedUri)
        {
            if (uris.IsNullOrEmpty()) return false;

            return uris.Contains(requestedUri, StringComparer.OrdinalIgnoreCase);
        }
    }
}