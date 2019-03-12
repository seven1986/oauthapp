using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseIdentityServer4MicroService(
           this IApplicationBuilder builder)
        {
            builder.Validate();

            builder.UseCors("cors-allowanonymous");

            var Configuration = builder.ApplicationServices.GetService<IConfiguration>();

            AppDefaultData.InitializeDatabase(builder, Configuration);

            builder.UseMiddleware<TenantMiddleware>();

            builder.UseAuthentication();

            builder.UseIdentityServer();

            return builder;
        }

        internal static void Validate(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            var logger = loggerFactory.CreateLogger("IdentityServer4.MicroService.Startup");

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                TestService(serviceProvider, typeof(UserDbContext), logger,
                    "No storage mechanism for Users specified. Use the 'AddIdentityStore' extension method to register a development version.");

                TestService(serviceProvider, typeof(TenantDbContext), logger,
                    "No storage mechanism for Tenants specified. Use the 'AddTenantStore' extension method to register a development version.");
            }
        }

        internal static object TestService(IServiceProvider serviceProvider,
            Type service, ILogger logger, string message = null, bool doThrow = true)
        {
            var appService = serviceProvider.GetService(service);

            if (appService == null)
            {
                var error = message ?? $"Required service {service.FullName} is not registered in the DI container. Aborting startup";

                logger.LogCritical(error);

                if (doThrow)
                {
                    throw new InvalidOperationException(error);
                }
            }

            return appService;
        }
    }
}
