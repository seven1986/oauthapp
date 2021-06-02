using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.DbContexts;
using OAuthApp;
using OAuthApp.Data;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using static OAuthApp.AppDefaultData;
using Microsoft.OpenApi.Models;
using AspNetCoreRateLimit;
using IdentityServer4.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class TenantExtensions
    {
        public static IApplicationBuilder UseOAuthApp(
           this IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService<IWebHostEnvironment>();

            var Configuration = builder.ApplicationServices.GetService<IConfiguration>();

            var options = builder.ApplicationServices.GetService<OAuthAppOptions>();

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

            if (options.EnableIpRateLimit || options.EnableClientRateLimit)
            {
                builder.UseClientRateLimiting();
            }

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
                    x.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                    });

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
                            c.OAuthUsePkce();
                            c.OAuthAppName(SwaggerClient.ClientName);
                            c.OAuthClientId(SwaggerClient.ClientId);
                            c.OAuthClientSecret(SwaggerClient.ClientSecret);
                            c.OAuth2RedirectUrl($"{options.IdentityServerUri.OriginalString}/swagger/oauth2-redirect.html");
                        }

                        c.DocExpansion(DocExpansion.None);

                        c.EnableValidator();
                    });
            }

            if (options.EnableReDoc)
            {
                builder.UseReDoc(c =>
                {
                    c.RoutePrefix = "docs";
                    c.SpecUrl("/swagger/v1/swagger.json");
                    c.EnableUntrustedSpec();
                    c.ScrollYOffset(10);
                    c.HideHostname();
                    c.HideDownloadButton();
                    c.ExpandResponses("200,201");
                    c.RequiredPropsFirst();
                    c.HideLoading();                  
                    c.DisableSearch();
                    c.SortPropsAlphabetically();

                    //c.OnlyRequiredInSamples();
                    //c.NoAutoAuth();
                    //c.PathInMiddlePanel();
                    //c.NativeScrollbars();

                    if (options.ReDocOptions!=null)
                    {
                        options.ReDocOptions.Invoke(c);
                    }
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

        /// <summary>
        /// 使用OAuthApp UI
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseOAuthAppUI(
         this IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService<IWebHostEnvironment>();

            var OAuthAppUI_DirectoryPath = $"{env.WebRootPath}/tenant";

            var OAuthAppUI_PackageFilePath = Path.Combine($"{env.WebRootPath}", "oauthapp-ui.zip");

            if (!Directory.Exists(OAuthAppUI_DirectoryPath))
            {
                using (var ms = new MemoryStream(AppResource.oauthapp_ui))
                {
                    using (var fs = new FileStream(OAuthAppUI_PackageFilePath,FileMode.CreateNew))
                    {
                        fs.Write(AppResource.oauthapp_ui, 0, AppResource.oauthapp_ui.Length);
                    }
                }

                new Func<bool>(() =>
                {
                    ZipFile.ExtractToDirectory(OAuthAppUI_PackageFilePath, env.WebRootPath);

                    return true;

                }).Invoke();

                File.Delete(OAuthAppUI_PackageFilePath);
            }


            var idsOptions = builder.ApplicationServices.GetService<IdentityServerOptions>();

            idsOptions.UserInteraction.LoginUrl = "/tenant/auth2/signin";
            idsOptions.UserInteraction.DeviceVerificationUrl = "/tenant/auth2/signin";
            idsOptions.UserInteraction.LogoutUrl = "/tenant/auth2/logout";
            idsOptions.UserInteraction.ErrorUrl = "/tenant/auth2/error";
            idsOptions.UserInteraction.ConsentUrl = "/tenant/auth2/consent";

            builder.Map("/tenant", subApp =>
            {
                subApp.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "/tenant";
                    spa.Options.DefaultPage = "/tenant/index.html";
                });

                builder.UseSpaStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider($"{env.WebRootPath}/tenant"),
                    RequestPath = "/tenant"
                });
            });

            builder.Map("/tenant-admin", subApp =>
            {
                subApp.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "/tenant-admin";
                    spa.Options.DefaultPage = "/tenant-admin/index.html";
                });

                builder.UseSpaStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider($"{env.WebRootPath}/tenant-admin"),
                    RequestPath = "/tenant-admin"
                });
            });

            return builder;
        }
        internal static void Validate(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            var logger = loggerFactory.CreateLogger("OAuthApp.Startup");

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
