using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OAuthApp.Tenant
{
    public class TenantMiddleware
    {
        readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var _tenantService = context.RequestServices.GetService<TenantProvider>();

            var tenantContext = _tenantService.GetTenantContext(context.Request.Host.Value);

            context.Items[TenantDefaults.CacheKey] = tenantContext;

            return _next.Invoke(context);
        }
    }
}
