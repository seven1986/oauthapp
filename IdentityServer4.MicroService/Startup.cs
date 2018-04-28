using System;
using System.IO;
using System.Linq;
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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Authentication.Weixin;
using Microsoft.AspNetCore.Authentication.Weibo;
using Microsoft.AspNetCore.Authentication.GitHub;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using AspNet.Security.OAuth.Amazon;
using AspNet.Security.OAuth.LinkedIn;
using AspNet.Security.OAuth.Instagram;
using AspNet.Security.OAuth.Paypal;
using AspNet.Security.OAuth.Gitter;
using AspNet.Security.OAuth.Reddit;
using AspNet.Security.OAuth.VisualStudio;
using AspNet.Security.OAuth.WordPress;
using AspNet.Security.OAuth.Salesforce;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using static IdentityServer4.MicroService.AppConstant;
using static IdentityServer4.MicroService.MicroserviceConfig;

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
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(DBConnection));

            services.AddIdentity<AppUser, AppRole>(opts =>
            {
                opts.SignIn.RequireConfirmedEmail = true;
                //opts.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
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

            AddOAuths(authBuilder);
            #endregion

            //for now, no need
            //var RedisConnection = Configuration["ConnectionStrings:RedisConnection"];
            //if (!string.IsNullOrWhiteSpace(RedisConnection))
            //{
            //    services.AddDistributedRedisCache(options =>
            //    {
            //        options.Configuration = RedisConnection;
            //        options.InstanceName = assemblyName + ":";
            //    });
            //}

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

            services.AddScoped<IPasswordHasher<AppUser>, IdentityMD5PasswordHasher>();
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
            services.Configure<SmsSenderOptions>(Configuration.GetSection("MessageSender:Sms"));
            services.Configure<EmailSenderOptions>(Configuration.GetSection("MessageSender:Email"));
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddTransient<EmailService>();
            #endregion

            services.AddSingleton<RedisService>();
            services.AddSingleton<TenantService>();
            services.AddSingleton<SwaggerCodeGenService>();
            services.AddSingleton<AzureStorageService>();
            services.AddScoped<ApiLoggerService>();

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
                            policy => policy.RequireAssertion(context =>
                            {
                                var userPermissionClaim = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.UserPermission));

                                if (userPermissionClaim != null && !string.IsNullOrWhiteSpace(userPermissionClaim.Value))
                                {
                                    var userPermissionClaimValue = userPermissionClaim.Value.ToLower().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                                    if (userPermissionClaimValue != null && userPermissionClaimValue.Length > 0)
                                    {
                                        foreach (var userPermissionItem in userPermissionClaimValue)
                                        {
                                            if (permissionValues.Contains(userPermissionItem))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }

                                return false;
                            }));
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

            services.AddNodeServices(options => {
                // Set any properties that you want on 'options' here
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider provider)
        {
            AppDefaultData.InitializeDatabase(app,Configuration);

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

            app.UseMutitenancy();

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
            #endregion
        }

        void AddOAuths(AuthenticationBuilder authBuilder)
        {
            #region Amazon (/signin-amazon)
            authBuilder.AddAmazon(x =>
            {
                var ClientId = $"{AmazonAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{AmazonAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Facebook (/signin-facebook)
            authBuilder.AddFacebook2(x =>
            {
                var ClientId = $"{FacebookDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{FacebookDefaults.AuthenticationScheme}:ClientSecret";
                x.AppId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.AppSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region GitHub (/signin-github)
            authBuilder.AddGitHub(x =>
            {
                var ClientId = $"{GitHubDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GitHubDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Gitter (/signin-gitter)
            authBuilder.AddGitter(x =>
            {
                var ClientId = $"{GitterAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GitterAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Google (/signin-google)
            authBuilder.AddGoogle2(x =>
            {
                var ClientId = $"{GoogleDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{GoogleDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Instagram (/signin-instagram)
            authBuilder.AddInstagram(x =>
            {
                var ClientId = $"{InstagramAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{InstagramAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region LinkedIn (/signin-linkedin)
            authBuilder.AddLinkedIn(x =>
            {
                var ClientId = $"{LinkedInAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{LinkedInAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region MicrosoftAccount (/signin-microsoft)
            authBuilder.AddMicrosoftAccount2(x =>
            {
                var ClientId = $"{MicrosoftAccountDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{MicrosoftAccountDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Paypal (/signin-paypal)
            authBuilder.AddPaypal(x =>
            {
                var ClientId = $"{PaypalAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{PaypalAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region QQ (/signin-qq)
            authBuilder.AddQQ(x =>
            {
                var ClientId = $"{QQDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{QQDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Reddit (/signin-reddit)
            authBuilder.AddReddit(x =>
            {
                var ClientId = $"{RedditAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{RedditAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Salesforce (/signin-salesforce)
            authBuilder.AddSalesforce(x =>
            {
                var ClientId = $"{SalesforceAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{SalesforceAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Twitter (/signin-twitter)
            authBuilder.AddTwitter2(x =>
            {
                var ClientId = $"{TwitterDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{TwitterDefaults.AuthenticationScheme}:ClientSecret";
                x.ConsumerKey = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ConsumerSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region VisualStudio (/signin-visualstudio)
            authBuilder.AddVisualStudio(x =>
            {
                var ClientId = $"{VisualStudioAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{VisualStudioAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Weibo (/signin-weibo)
            authBuilder.AddWeibo(x =>
            {
                var ClientId = $"{WeiboDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WeiboDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region Weixin (/signin-weixin)
            authBuilder.AddWeixin(x =>
            {
                var ClientId = $"{WeixinDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WeixinDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion

            #region WordPress (/signin-visualstudio)
            authBuilder.AddWordPress(x =>
            {
                var ClientId = $"{WordPressAuthenticationDefaults.AuthenticationScheme}:ClientId";
                var ClientSecret = $"{WordPressAuthenticationDefaults.AuthenticationScheme}:ClientSecret";
                x.ClientId = AppDefaultData.Tenant.TenantProperties[ClientId];
                x.ClientSecret = AppDefaultData.Tenant.TenantProperties[ClientSecret];
            });
            #endregion
        }
    }
}
