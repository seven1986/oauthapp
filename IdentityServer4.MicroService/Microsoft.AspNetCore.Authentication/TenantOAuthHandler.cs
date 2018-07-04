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
            Options.ClientId = AppDefaultData.Tenant.TenantProperties[$"{Scheme.Name}:ClientId"];

            Options.ClientSecret = AppDefaultData.Tenant.TenantProperties[$"{Scheme.Name}:ClientSecret"];

            return base.InitializeHandlerAsync();
        }
    }
}
