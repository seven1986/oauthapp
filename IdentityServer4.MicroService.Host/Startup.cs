using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Data;
using Swashbuckle.AspNetCore.Swagger;
using IdentityServer4.MicroService.Host.Filters;
using Microsoft.Net.Http.Headers;

namespace IdentityServer4.MicroService.Host
{
    public class Startup
    {
        IHostingEnvironment _env;

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            
            builder.AddEnvironmentVariables();

            var config = builder.Build();

            var AzureKeyVaultSection = config.GetSection("AzureKeyVault");

            if (AzureKeyVaultSection.Exists())
            {
                var VaultBaseUrl = AzureKeyVaultSection["VaultBaseUrl"];

                var VaultClientId = AzureKeyVaultSection["ClientId"];

                var VaultClientSecret = AzureKeyVaultSection["ClientSecret"];

                if (!string.IsNullOrWhiteSpace(VaultBaseUrl) &&
                    !string.IsNullOrWhiteSpace(VaultClientId) &&
                    !string.IsNullOrWhiteSpace(VaultClientSecret))
                {
                    builder.AddAzureKeyVault(VaultBaseUrl, VaultClientId, VaultClientSecret);
                }
            }

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            #region Cors
            services.AddCors(options =>
            {
                options.AddPolicy("default", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin(); 
                    //builder.AllowCredentials();
                });
            });
            #endregion

            #region Authentication & OAuth
            //Authentication 
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            //IdentityServerAuthentication
            .AddIdentityServerAuthentication(AppConstant.AppAuthenScheme, isAuth =>
            {
                isAuth.Authority = "https://" + Configuration["IdentityServer"];
                isAuth.ApiName = MicroserviceConfig.MicroServiceName;
                isAuth.RequireHttpsMetadata = true;
            })
            //OAuths Login
            .AddIdentityServer4MicroServiceOAuths();
            #endregion

            #region Mvc + localization
            // Configure supported cultures and localization options
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture("zh-CN", "zh-CN");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;

                // You can change which providers are configured to determine the culture for requests, or even add a custom
                // provider with your own logic. The providers will be asked in order to provide a culture for each request,
                // and the first to provide a non-null result that is in the configured supported cultures list will be used.
                // By default, the following built-in providers are configured:
                // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
                // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
                // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header
                //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
                //{
                //  // My custom request culture logic
                //  return new ProviderCultureResult("en");
                //}));
            });

            //https://github.com/Microsoft/aspnet-api-versioning/wiki/API-Documentation#aspnet-core
            services.AddMvcCore().AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

            services.AddMvc(options =>
            {
                // for external authentication,maybe not need
                //options.SslPort = 44314;
                // for production, microsoft authentication need https
                options.Filters.Add(new RequireHttpsAttribute());
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            //https://stackoverflow.com/questions/34753498/self-referencing-loop-detected-in-asp-net-core
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddApiVersioning(o => {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ReportApiVersions = true;
            });
            #endregion

            #region SwaggerGen
            services.AddSwaggerGen(c =>
            {
                // c.TagActionsBy(x => x.RelativePath.Split('/')[0]);

                c.AddSecurityDefinition("SubscriptionKey",
                    new ApiKeyScheme()
                    {
                        Name = "Ocp-Apim-Subscription-Key",
                        Type = "apiKey",
                        In = "header",
                        Description = "从开放平台申请的Subscription Key，从网关调用接口时必需传入。",
                    });

                c.AddSecurityDefinition("AccessToken",
                    new ApiKeyScheme()
                    {
                        Name = "Authorization",
                        Type = "apiKey",
                        In = "header",
                        Description = "从身份认证中心颁发的Token，根据接口要求决定是否传入。",
                    });

                c.AddSecurityDefinition("OAuth2",
                    new OAuth2Scheme()
                    {
                        Type = "oauth2",
                        Flow = "accessCode",
                        AuthorizationUrl = "https://" + Configuration["IdentityServer"] + "/connect/authorize",
                        TokenUrl = "https://" + Configuration["IdentityServer"] + "/connect/token",
                        Description = "勾选授权范围，获取Token",
                        Scopes = new Dictionary<string, string>(){
                            { "openid","用户标识" },
                            { "profile","用户资料" },
                            { MicroserviceConfig.MicroServiceName+ ".all","所有接口权限"},
                        }
                    });

                c.OperationFilter<FormFileOperationFilter>();

                var provider = services.BuildServiceProvider()
                               .GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new Info
                    {
                        Title = assemblyName,
                        Version = description.ApiVersion.ToString(),
                        License = new License()
                        {
                            Name = "MIT",
                            Url = "https://spdx.org/licenses/MIT.html"
                        },
                        // Contact = new Contact()
                        // {
                        //     Url = "",
                        //     Name = "",
                        //     Email = ""
                        // },
                        // Description = "Swagger document",
                    });
                }

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, assemblyName + ".xml");

                c.IncludeXmlComments(filePath);

                c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "IdentityServer4.MicroService.xml"));
            });
            #endregion

            #region DbContextOptions
            var DBConnection = Configuration["ConnectionStrings:DataBaseConnection"];
            var DbContextOptions = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnection,
            opts => opts.MigrationsAssembly(assemblyName)));
            #endregion

            #region IdentityServer4MicroService
            services.AddIdentityServer4MicroService(Configuration)
                    .AddTenantStore(DbContextOptions)
                    .AddIdentityStore(DbContextOptions, opts =>
                    {
                        //opts.SignIn.RequireConfirmedEmail = true;
                    });
            #endregion

            #region IdentityServer
            var cert = GetSigningCredential();
            services.AddIdentityServer(config =>
            {
                // keep same Issuer for banlancer
                config.IssuerUri = "https://" + Configuration["IdentityServer"];
                // config.PublicOrigin = "";
                // config.Discovery.CustomEntries.Add("custom_endpoint", "~/api/custom");
            })
              .AddSigningCredential(cert)
              .AddCustomAuthorizeRequestValidator<TenantAuthorizeRequestValidator>()
              .AddCustomTokenRequestValidator<TenantTokenRequestValidator>()
              .AddConfigurationStore(builder => builder.ConfigureDbContext = DbContextOptions)
              .AddOperationalStore(builder => builder.ConfigureDbContext = DbContextOptions)
              .AddAspNetIdentity<AppUser>();
            #endregion

            services.AddNodeServices(options => {
                // Set any properties that you want on 'options' here
            });
        }

        X509Certificate2 GetSigningCredential()
        {
            X509Certificate2 cert = null;

            var AzureKeyVaultSection = Configuration.GetSection("AzureKeyVault");

            if (AzureKeyVaultSection.Exists())
            {
                var VaultBaseUrl = AzureKeyVaultSection["VaultBaseUrl"];
                var certificateName = AzureKeyVaultSection["Certificate:Name"];
                var certificateVersion = AzureKeyVaultSection["Certificate:Version"];

                if (!string.IsNullOrWhiteSpace(VaultBaseUrl) &&
                    !string.IsNullOrWhiteSpace(certificateName) &&
                    !string.IsNullOrWhiteSpace(certificateVersion))
                {
                    var getToken = new Func<string, string, string, Task<string>>((authority, resource, scope) =>
                    {
                        var clientCred = new ClientCredential(
                            Configuration["AzureKeyVault:ClientId"],
                            Configuration["AzureKeyVault:ClientSecret"]);

                        var authContext = new AuthenticationContext(authority, true);

                        var result = authContext.AcquireTokenAsync(resource, clientCred).Result;

                        if (result == null)
                            throw new InvalidOperationException("Failed to obtain the JWT token");

                        return Task.FromResult(result.AccessToken);
                    });

                    using (var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(getToken)))
                    {
                        // 有公钥的证书
                        var CertificateWithPubKey = kvClient.GetCertificateAsync(VaultBaseUrl,
                            certificateName, certificateVersion).Result;

                        // 有私钥的证书
                        var CertificateWithPrivateKey = kvClient.GetSecretAsync(CertificateWithPubKey.SecretIdentifier.Identifier).Result;

                        // 默认用的是UserKeySet，但在Azure Web App里需要MachineKeySet
                        cert = new X509Certificate2(Convert.FromBase64String(CertificateWithPrivateKey.Value),
                            string.Empty,
                            X509KeyStorageFlags.MachineKeySet);

                    }
                }
            }

            if (cert == null)
            {
                var IdentityServerCertificateSection = Configuration.GetSection("IdentityServerCertificate");

                if (IdentityServerCertificateSection.Exists())
                {
                    var certFilePath = IdentityServerCertificateSection["FilePath"];
                    var certPassword = IdentityServerCertificateSection["CertPassword"];

                    if (!string.IsNullOrWhiteSpace(certFilePath) &&
                        !string.IsNullOrWhiteSpace(certPassword))
                    {
                        var certPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, certFilePath);

                        cert = new X509Certificate2(certPath, certPassword,
                            X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
                    }
                }
            }

            return cert;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            AppDefaultData.InitializeDatabase(app, Configuration);

            app.UseCors("default");

            #region Localization
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            #endregion

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
                app.UseSwaggerUI(c =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());

                        c.OAuthClientId(AppDefaultData.TestClient.ClientId);
                        c.OAuthClientSecret(AppDefaultData.TestClient.ClientSecret);
                        c.OAuthAppName(AppDefaultData.TestClient.ClientName);
                        c.OAuth2RedirectUrl(AppDefaultData.TestClient.RedirectUris[0]);
                    }

                    c.DocExpansion(DocExpansion.None);
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                    "public,max-age=" + durationInSeconds;
                }
            });

            // do not change the order here
            app.UseIdentityServer4MicroService();

            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();

            #region swagger
            app.UseSwagger(x =>
            {
                x.PreSerializeFilters.Add((doc, req) =>
                {
                    doc.Schemes = new[] { "https" };
                    doc.Host = Configuration["IdentityServer"];
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
            #endregion
        }
    }
}
