using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OAuthApp.Models.Shared;
using System.Security.Claims;

namespace OAuthApp.Tenant
{
    public static class TenantValidatorHelper
    {
        public static Claim GetTenantTokenClaim(this HttpContext _context)
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

        public static TenantPrivateModel GetTenantWithClaims(this HttpContext _context)
        {
            var TenantContext = _context.Items[TenantConstant.CacheKey];

            if (TenantContext != null)
            {
                return TenantContext as TenantPrivateModel;
            }

            return null;
        }

        public static TenantPublicModel GetTenantWithProperties(this HttpContext _context)
        {
            var TenantContext = _context.Items[TenantConstant.HttpContextItemKey];

            if (TenantContext != null)
            {
                return TenantContext as TenantPublicModel;
            }

            return null;
        }
    }
}
