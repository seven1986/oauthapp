using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OAuthApp.Tenant
{
    public class TenantAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        HttpContext _context;

        public TenantAuthorizeRequestValidator(IHttpContextAccessor contextAccessor)
        {
            _context = contextAccessor.HttpContext;
        }

        public Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            if (!context.Result.IsError)
            {
                var TenantClaim = _context.GetTenantTokenClaim();

                if (TenantClaim != null)
                {
                    context.Result.ValidatedRequest.ClientClaims.Add(TenantClaim);
                }
            }

            return Task.CompletedTask;
        }
    }
}
