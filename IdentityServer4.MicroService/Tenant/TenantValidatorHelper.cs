using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace OAuthApp.Tenant
{
    public static class TenantValidatorHelper
    {
        public static Claim GetTenantClaim(this HttpContext _context)
        {
            var TenantContext = _context.Items[TenantConstant.CacheKey];

            if (TenantContext != null)
            {
                var TenantContextString = JsonConvert.SerializeObject(TenantContext);

                return new Claim(
                    TenantConstant.TokenKey,
                    TenantContextString,
                    IdentityServerConstants.ClaimValueTypes.Json);
            }

            return null;
        }
    }
}
