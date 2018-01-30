using IdentityServer4.MicroService.Tenant;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseMutitenancy(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
