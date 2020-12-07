using OAuthApp.Models.Apis.AuthingController;
using System.Collections.Generic;

namespace OAuthApp.Models.Apis.ConsentController
{
   public class AuthingScopesReponse
    {
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public IEnumerable<AuthingScopeItem> IdentityScopes { get; set; }
        public IEnumerable<AuthingScopeItem> ApiScopes { get; set; }
    }
}
