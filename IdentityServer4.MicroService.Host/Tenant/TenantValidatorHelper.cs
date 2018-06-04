using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using IdentityServer4;

namespace IdentityServer4.MicroService.Tenant
{
    public static class TenantValidatorHelper
    {
        public static Claim GetTenantClaim(this HttpContext _context)
        {
            var TenantContext = _context.Items[TenantConstant.CacheKey];

            if (TenantContext != null)
            {
                var TenantContextString = TenantContext.ToString();

                return new Claim(
                    TenantConstant.TokenKey,
                    TenantContextString,
                    IdentityServerConstants.ClaimValueTypes.Json);
            }

            return null;
        }
    }
}
