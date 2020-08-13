using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using static IdentityServer4.MicroService.AppDefaultData;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseIdentityServer4MicroService(
           this IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService<IWebHostEnvironment>();

            var Configuration = builder.ApplicationServices.GetService<IConfiguration>();

            var options = builder.ApplicationServices.GetService<IdentityServer4MicroServiceOptions>();

            if (options.IdentityServerUri == null)
            {
                try
                {
                    options.IdentityServerUri = new Uri(Configuration["IdentityServer:Host"]);
                }
                catch
                {
                    throw new KeyNotFoundException("appsettings.json文件，没有配置IdentityServer:Host");
                }
            }

            builder.Validate();

            if (options.EnableCors)
            {
                builder.UseCors("cors-allowanonymous");
            }

            if (AppConstant.InitializeDatabase)
            {
                InitializeDatabase(builder, options);
            }

            builder.UseMiddleware<TenantMiddleware>();

            builder.UseAuthentication();

            builder.UseIdentityServer();

            builder.UseAuthorization();

            if (options.EnableLocalization)
            {
                var locOptions = builder.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();

                builder.UseRequestLocalization(locOptions.Value);
            }

            if (options.EnableSwaggerGen)
            {
                builder.UseSwagger(x =>
                {
                    //x.PreSerializeFilters.Add((doc, req) =>
                    //{
                    //    doc.Schemes = new[] { "https" };
                    //    doc.Host = options.IdentityServerUri.Authority;
                    //    doc.Security = new List<IDictionary<string, IEnumerable<string>>>()
                    //    {
                    //        new Dictionary<string, IEnumerable<string>>()
                    //        {
                    //            { "SubscriptionKey", new string[]{ } },
                    //            { "AccessToken", new string[]{ } },
                    //            { "OAuth2", new string[]{ } },
                    //        }
                    //    };
                    //});
                });
            }

            if (options.EnableSwaggerUI)
            {
                builder.UseSwaggerUI(c =>
                    {
                        var provider = builder.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            c.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());

                            c.OAuthAppName(SwaggerClient.ClientName);
                            c.OAuthClientId(SwaggerClient.ClientId);
                            c.OAuthClientSecret(SwaggerClient.ClientSecret);
                            c.OAuth2RedirectUrl($"{options.IdentityServerUri.OriginalString}/swagger/oauth2-redirect.html");
                        }

                        c.DocExpansion(DocExpansion.None);

                        c.EnableValidator();
                    });
            }

            if (options.EnableResponseCaching)
            {
                builder.UseResponseCaching();
            }

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

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

                TestService(serviceProvider, typeof(ConfigurationDbContext), logger,
                    "No storage mechanism for Users specified. Use the 'AddIdentityStore' extension method to register a development version.");


                TestService(serviceProvider, typeof(PersistedGrantDbContext), logger,
                    "No storage mechanism for Users specified. Use the 'AddIdentityStore' extension method to register a development version.");

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
