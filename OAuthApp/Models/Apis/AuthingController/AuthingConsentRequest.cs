using System.Collections.Generic;

namespace OAuthApp.Models.Apis.ConsentController
{
   public class AuthingConsentRequest
    {
        public string Button { get; set; }
        public List<string> ScopesConsented { get; set; }
        public bool RememberConsent { get; set; }
        public string ReturnUrl { get; set; }
        public string Description { get; set; }
    }
}
