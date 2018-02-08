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
using Microsoft.AspNetCore.Identity;
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
using Swashbuckle.AspNetCore.Swagger;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using ApiTracker;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService
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

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var clientCred = new ClientCredential(
                Configuration["AzureKeyVault:ClientId"],
                Configuration["AzureKeyVault:ClientSecret"]);

                var authContext = new AuthenticationContext(authority, true);

                var result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Cors
            services.AddCors(options =>
                {
                    options.AddPolicy("default", builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowAnyOrigin();
                        builder.AllowCredentials();
                    });
                }); 
            #endregion

            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connSection = Configuration.GetSection("ConnectionStrings");
            var DBConnection = connSection["DataBaseConnection"];

            #region DbContext
            // Add TenantDbContext.
            services.AddDbContext<TenantDbContext>(options =>
                options.UseSqlServer(DBConnection));

            // Add ApplicationDbContext.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DBConnection));

            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = true;
                //opts.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.Configure<ConnectionStrings>(connSection);
            #endregion

            #region 联合登陆
            // https://github.com/aspnet/Security/issues/1576
            // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/microsoft-logins
            // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/facebook-logins
            // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/google-logins
            // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/social/twitter-logins
            var authBuilder = services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            });

            authBuilder.AddIdentityServerAuthentication(AppAuthenScheme, isAuth =>
            {
                isAuth.Authority = "https://" + Configuration["IdentityServer"];
                isAuth.ApiName = MicroServiceName;
                isAuth.RequireHttpsMetadata = true;
            });

            //authBuilder.AddWeixin(x =>
            //{
            //    x.ClientId = Configuration["Authentication:Weixin:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:Weixin:ClientSecret"];
            //});

            //authBuilder.AddWeibo(x =>
            //{
            //    x.ClientId = Configuration["Authentication:Weibo:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:Weibo:ClientSecret"];
            //});

            //authBuilder.AddGitHub(x =>
            //{
            //    x.ClientId = Configuration["Authentication:GitHub:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
            //});

            //authBuilder.AddQQ(x =>
            //{
            //    x.ClientId = Configuration["Authentication:QQ:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:QQ:ClientSecret"];
            //});

            //authBuilder.AddFacebook(x =>
            //{
            //    x.AppId = Configuration["Authentication:Facebook:ClientId"];
            //    x.AppSecret = Configuration["Authentication:Facebook:ClientSecret"];
            //});

            //authBuilder.AddTwitter(x =>
            //{
            //    x.ConsumerKey = Configuration["Authentication:Twitter:ClientId"];
            //    x.ConsumerSecret = Configuration["Authentication:Twitter:ClientSecret"];
            //});

            //authBuilder.AddGoogle(x =>
            //{
            //    x.ClientId = Configuration["Authentication:Google:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});

            //authBuilder.AddMicrosoftAccount(x =>
            //{
            //    x.ClientId = Configuration["Authentication:Microsoft:ClientId"];
            //    x.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            //});
            #endregion

            // Common Cache Service, for now, no need
            // services.AddDistributedRedisCache(options => {
            //    options.Configuration = Configuration["ConnectionStrings:RedisConnection"];
            //    options.InstanceName = assemblyName;
            // });

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
                            { MicroServiceName+ ".all","所有接口权限"},
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
            });
            #endregion

            #region MessageSender
            services.Configure<SmsSenderOptions>(Configuration.GetSection("MessageSender:sms"));
            services.Configure<EmailSenderOptions>(Configuration.GetSection("MessageSender:Email"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            #endregion

            services.AddTransient(typeof(SqlService));
            services.AddTransient(typeof(AzureStorageService));
            services.AddSingleton<RedisService>();
            services.AddSingleton<TenantService>();
            services.AddSingleton<SwaggerCodeGenService>();
            //services.AddTransient<AzureApiManagementServices>();

            #region 权限定义
            services.AddAuthorization(options =>
                {
                    #region Client的权限策略
                    var scopes = typeof(ClientScopes).GetFields();

                    foreach (var scope in scopes)
                    {
                        var scopeName = scope.GetRawConstantValue().ToString();

                        var scopeValues = scope.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues;

                        options.AddPolicy(scopeName,policy => policy.RequireClaim(ClaimTypes.ClientScope, scopeValues));
                    }
                    #endregion

                    #region User的权限策略
                    var permissions = typeof(UserPermissions).GetFields();

                    foreach (var permission in permissions)
                    {
                        var permissionName = permission.GetRawConstantValue().ToString();

                        var permissionValues = permission.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues;

                        options.AddPolicy(permissionName,
                            policy => policy.RequireClaim(ClaimTypes.UserPermission, permissionValues));
                    }
                    #endregion
                });
            #endregion

            #region IdentityServer
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
                    using (var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken)))
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

            if(cert==null)
            {
                var IdentityServerCertificateSection = Configuration.GetSection("IdentityServerCertificate");

                if (IdentityServerCertificateSection.Exists())
                {
                    var certFilePath = IdentityServerCertificateSection["FilePath"];
                    var certPassword = IdentityServerCertificateSection["CertPassword"];

                    if (!string.IsNullOrWhiteSpace(certFilePath) && 
                        !string.IsNullOrWhiteSpace(certPassword))
                    {
                        var certPath = _env.WebRootPath + "\\" + certFilePath;
                        cert = new X509Certificate2(certPath, certPassword,
                            X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
                    }
                }
            }

            var IdentityServerStore = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnection,
            opts => opts.MigrationsAssembly(assemblyName)));

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
              .AddConfigurationStore(builder => builder.ConfigureDbContext = IdentityServerStore)
              .AddOperationalStore(builder => builder.ConfigureDbContext = IdentityServerStore)
              .AddAspNetIdentity<AppUser>();
            #endregion

            services.Configure<ApiTrackerSetting>(Configuration.GetSection("ApiTrackerSetting"));
            services.AddScoped<ApiTracker.ApiTracker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            InitialDBConfig.InitializeDatabase(app);

            app.UseMutitenancy();

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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();

            #region swagger
            app.UseSwagger(x =>
                {
                    x.PreSerializeFilters.Add((doc, req) =>
                    {
                        doc.Schemes = new[] { "https" };
                        //doc.Host = Configuration["IdentityServer"];
                    });
                });
            #endregion

            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());

                    c.ConfigureOAuth2("test", "1", string.Empty, "API测试专用");
                }

                c.DocExpansion("none");
            });   
        }
    }
}
