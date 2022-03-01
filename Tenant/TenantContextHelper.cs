using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace OAuthApp.Tenant
{
    public static class TenantContextHelper
    {
        public static TenantContext GetTenantContext(this HttpContext _context)
        {
            var tenantContext = _context.Items[TenantDefaults.CacheKey];

            if (tenantContext != null)
            {
                return tenantContext as TenantContext;
            }

            return null;
        }

        public static AuthenticationTicket CreateTicket(this TenantContext tenant,
            TenantUserModel user,
            string clientId)
        {
            var claims = CreateClaims(tenant, user, clientId);

            var claimsIdentity = new ClaimsIdentity(claims, clientId);

            var result = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), clientId);

            return result;
        }

        public static List<Claim> CreateClaims(this TenantContext tenant,
            TenantUserModel user,
            string clientId)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(TenantClaimTypes.TenantId, tenant.Id.ToString()));
            claims.Add(new Claim(TenantClaimTypes.TenantName, tenant.Name));
            claims.Add(new Claim(TenantClaimTypes.ClientId, clientId));
            claims.Add(new Claim(TenantClaimTypes.Mobile, user.Mobile));
            claims.Add(new Claim(TenantClaimTypes.Picture, user.Avatar??""));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Role, user.Role));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.GivenName, user.NickName));

            return claims;
        }
    }
}
