using AspNetCoreRateLimit;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;

namespace OAuthApp.Configuration
{
    public class OAuthAppRateLimitConfiguration : RateLimitConfiguration
    {
        public OAuthAppRateLimitConfiguration(IHttpContextAccessor httpContextAccessor, 
            IOptions<IpRateLimitOptions> ipOptions, 
            IOptions<ClientRateLimitOptions> clientOptions) : base(httpContextAccessor, ipOptions, clientOptions) { }

        protected override void RegisterResolvers()
        {
            base.RegisterResolvers();

            ClientResolvers.Add(
                new OAuthAppClientResolveContributor(HttpContextAccessor));
        }
    }

    public class OAuthAppClientResolveContributor : IClientResolveContributor
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        const string AnonymousClient = "AnonymousClient";

        public OAuthAppClientResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ResolveClient()
        {
            var Identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (Identity != null)
            {
                //get clientId from accessToken
                var claim = Identity.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.ClientId));

                if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return claim.Value;
                }
            }

            return AnonymousClient;
        }
    }
}
