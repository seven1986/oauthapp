using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Tenant
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
                var TenantClaim = _context.GetTenantClaim();

                if (TenantClaim != null)
                {
                    context.Result.ValidatedRequest.ClientClaims.Add(TenantClaim);
                }
            }

            return Task.CompletedTask;
        }
    }
}
