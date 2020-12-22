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
            if (client.ClientId.Equals(AppConstant.MicroServiceName))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(StringCollectionContainsString(client.PostLogoutRedirectUris, requestedUri));
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client.ClientId.Equals(AppConstant.MicroServiceName))
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
