using System;

namespace OAuthApp.Tenant
{
    public class TokenModel
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public long expires_in { get; set; }
    }
}
