using AspNetCoreRateLimit;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OAuthApp.Configuration
{
    public class OAuthAppRateLimitConfiguration : RateLimitConfiguration
    {
        readonly HttpContextAccessor _accessor;
        public OAuthAppRateLimitConfiguration(HttpContextAccessor accessor,IOptions<IpRateLimitOptions> ipOptions, 
            IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions) {
            _accessor = accessor;
        }

        public override void RegisterResolvers()
        {
            base.RegisterResolvers();
            ClientResolvers.Add(
                new OAuthAppClientResolveContributor(_accessor));
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

        public Task<string> ResolveClientAsync(HttpContext httpContext)
        {
            var Identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (Identity != null)
            {
                //get clientId from accessToken
                var claim = Identity.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.ClientId));

                if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    return Task.FromResult(claim.Value);
                }
            }

            return Task.FromResult(AnonymousClient);
        }
    }
}
