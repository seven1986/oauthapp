using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseIdentityServer4MicroService(
           this IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService<IHostingEnvironment>();

            var Configuration = builder.ApplicationServices.GetService<IConfiguration>();

            var options = builder.ApplicationServices.GetService<IdentityServer4MicroServiceOptions>();

            if (options.IdentityServer == null)
            {
                options.IdentityServer = new Uri(Configuration["IdentityServer"]);
            }

            builder.Validate();

            if (options.EnableCors)
            {
                builder.UseCors("cors-allowanonymous");
            }

            if (options.InitializeDatabase)
            {
                AppDefaultData.InitializeDatabase(builder, options.MicroServiceName, options.IdentityServer);
            }

            builder.UseMiddleware<TenantMiddleware>();

            builder.UseAuthentication();

            builder.UseIdentityServer();

            if (options.SwaggerGen)
            {
                builder.UseSwagger(x =>
                {
                    x.PreSerializeFilters.Add((doc, req) =>
                    {
                        doc.Schemes = new[] { "https" };
                        doc.Host = options.IdentityServer.Authority;
                        doc.Security = new List<IDictionary<string, IEnumerable<string>>>()
                        {
                            new Dictionary<string, IEnumerable<string>>()
                            {
                                { "SubscriptionKey", new string[]{ } },
                                { "AccessToken", new string[]{ } },
                                { "OAuth2", new string[]{ } },
                            }
                        };
                    });
                });
            }

            if (options.SwaggerUI)
            {
                builder.UseSwaggerUI(c =>
                    {
                        var provider = builder.ApplicationServices.GetService<IApiVersionDescriptionProvider>();

                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            c.SwaggerEndpoint(
                                $"/swagger/{description.GroupName}/swagger.json",
                                description.GroupName.ToUpperInvariant());

                            c.OAuthAppName(options.SwaggerUIClientName);
                            c.OAuthClientId(options.SwaggerUIClientID);
                            c.OAuthClientSecret(options.SwaggerUIClientSecret);
                            c.OAuth2RedirectUrl($"{options.IdentityServer.ToString()}/swagger/oauth2-redirect.html");
                        }

                        c.DocExpansion(DocExpansion.None);
                    });
            }
            
            if (options.EnableResponseCaching)
            {
                builder.UseResponseCaching();
            }

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
