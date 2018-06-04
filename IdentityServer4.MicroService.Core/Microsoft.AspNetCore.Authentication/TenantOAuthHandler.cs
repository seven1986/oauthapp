using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Microsoft.AspNetCore.Authentication
{
    public class TenantOAuthHandler<T> : OAuthHandler<T> where T:OAuthOptions,new ()
    {
        public TenantOAuthHandler(
            IOptionsMonitor<T> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task InitializeHandlerAsync()
        {
            var ClientId = AppDefaultData.Tenant.TenantProperties[$"{Scheme.Name}:ClientId"];

            var ClientSecret = AppDefaultData.Tenant.TenantProperties[$"{Scheme.Name}:ClientSecret"];

            if (!string.IsNullOrWhiteSpace(ClientId) &&
               !string.IsNullOrWhiteSpace(ClientSecret))
            {
                Options.ClientId = ClientId;
                Options.ClientSecret = ClientSecret;
            }
            return base.InitializeHandlerAsync();
        }
    }
}
