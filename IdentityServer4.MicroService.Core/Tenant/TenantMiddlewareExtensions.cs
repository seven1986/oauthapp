using IdentityServer4.MicroService.Tenant;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseIdentityServer4MicroService(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
