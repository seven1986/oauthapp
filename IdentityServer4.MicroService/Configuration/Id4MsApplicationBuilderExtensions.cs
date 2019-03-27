using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Configuration;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Id4MsApplicationBuilderExtensions
    {
        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroServiceBuilder(this IServiceCollection services)
        {
            return new Id4MsServiceBuilder(services);
        }

        static List<PolicyConfig> PolicyConfigs(List<Type> types)
        {
            var policies = new List<PolicyConfig>();

            foreach (var type in types)
            {
                var policyObject = policies.FirstOrDefault(x => x.ControllerName.Equals(type.Name.ToLower()));

                if (policyObject == null)
                {
                    policyObject = new PolicyConfig()
                    {
                        ControllerName = type.Name.ToLower().Replace("controller", "")
                    };
                }

                var ControllerAttributes = type.GetMethods().Select(x => x.GetCustomAttributes<AuthorizeAttribute>()).ToList();

                foreach (var attr in ControllerAttributes)
                {
                    var ControllerPolicies = attr.Select(x => x.Policy.ToLower()).ToList();

                    if (ControllerPolicies.Count > 0)
                    {
                        var scopes = ControllerPolicies
                            .Where(x => x.IndexOf($"{PolicyKey.ClientScope}:") > -1).ToList();

                        scopes.ForEach(x =>
                        {
                            policyObject.Scopes.Add(x.Replace($"{PolicyKey.ClientScope}:", ""));
                        });

                        var permissions = ControllerPolicies
                            .Where(x => x.IndexOf($"{PolicyKey.UserPermission}:") > -1).ToList();

                        permissions.ForEach(x =>
                        {
                            policyObject.Permissions.Add(x.Replace($"{PolicyKey.UserPermission}:", ""));
                        });
                    }
                }

                policies.Add(policyObject);
            }
        
            return policies;
        }

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="ismsOptions">The Options.</param>
        /// <param name="configuration">The Configuration.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IdentityServer4MicroServiceOptions> ismsOptions = null)
        {
            var Options = new IdentityServer4MicroServiceOptions()
            {
                MicroServiceName = "ids4.ms",

                AssemblyName = Assembly.GetEntryAssembly().GetName().Name
            };

            if (Options != null)
            {
                ismsOptions.Invoke(Options);
            }

            var builder = services.AddIdentityServer4MicroServiceBuilder();
            builder.Services.AddSingleton(Options);

            #region Cors
            if (Options.EnableCors)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("cors-allowanonymous", x =>
                    {
                        x.AllowAnyHeader();
                        x.AllowAnyMethod();
                        x.AllowAnyOrigin();
                        x.AllowCredentials();
                    });
                });
            }
            #endregion

            #region WebEncoders
            if (Options.WebEncoders)
            {
                services.AddWebEncoders(opt =>
                {
                    opt.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });
            }
            #endregion

            #region AuthorizationPolicy
            if (Options.AuthorizationPolicy)
            {
                builder.Services.AddAuthorization(options =>
                {
                    var ISMSTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => x.BaseType != null && x.BaseType.Name.Equals("BasicController")).ToList();

                    var isms_policies = PolicyConfigs(ISMSTypes);

                    var EntryTypes = Assembly.GetEntryAssembly().GetTypes()
                       .Where(x => x.BaseType != null && x.BaseType.Name.Equals("ControllerBase")).ToList();

                    var entry_policies = PolicyConfigs(EntryTypes);

                    isms_policies.AddRange(entry_policies);

                    foreach (var policyConfig in isms_policies)
                    {
                        #region Client的权限策略
                        policyConfig.Scopes.ForEach(x =>
                        {
                            var policyName = $"{PolicyKey.ClientScope}:{x}";

                            var policyValues = new List<string>()
                            {
                                $"{Options.MicroServiceName}.{x}",
                                $"{Options.MicroServiceName}.{policyConfig.ControllerName}.all",
                                $"{Options.MicroServiceName}.all"
                            };

                            options.AddPolicy(policyName,
                                policy => policy.RequireClaim(PolicyKey.ClientScope, policyValues));
                        });
                        #endregion

                        #region User的权限策略
                        policyConfig.Permissions.ForEach(x =>
                        {
                            var policyName = $"{PolicyKey.UserPermission}:{x}";

                            var policyValues = new List<string>()
                            {
                                $"{Options.MicroServiceName}.{x}",
                                $"{Options.MicroServiceName}.{policyConfig.ControllerName}.all",
                                $"{Options.MicroServiceName}.all"
                            };

                            options.AddPolicy(policyName,
                                policy => policy.RequireAssertion(handler =>
                                {
                                    var claim = handler.User.Claims
                                    .FirstOrDefault(c => c.Type.Equals(PolicyKey.UserPermission));

                                    if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
                                    {
                                        var claimValues = claim.Value.ToLower().Split(new string[] { "," },
                                            StringSplitOptions.RemoveEmptyEntries);

                                        if (claimValues != null && claimValues.Length > 0)
                                        {
                                            foreach (var item in claimValues)
                                            {
                                                if (policyValues.Contains(item))
                                                {
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                    return false;
                                }));
                        });
                        #endregion
                    }
                });
            }
            #endregion

            #region SwaggerGen
            if (Options.SwaggerGen)
            {
                services.AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();

                    //c.TagActionsBy(x => x.RelativePath.Split('/')[0]);

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
                            AuthorizationUrl = Options.IdentityServer.OriginalString + "/connect/authorize",
                            TokenUrl = Options.IdentityServer.OriginalString + "/connect/token",
                            Description = "勾选授权范围，获取Token",
                            Scopes = new Dictionary<string, string>(){
                            { "openid","用户标识" },
                            { "profile","用户资料" },
                            { Options.MicroServiceName+ ".all","所有接口权限"},
                            }
                        });

                    var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerDoc(description.GroupName, new Info
                        {
                            Title = Options.AssemblyName,
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

                        c.CustomSchemaIds(x => x.FullName);
                    }

                    var SiteSwaggerFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, Options.AssemblyName + ".xml");

                    if (File.Exists(SiteSwaggerFilePath))
                    {
                        c.IncludeXmlComments(SiteSwaggerFilePath);
                    }

                    var ISMSSwaggerFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "IdentityServer4.MicroService.xml");

                    if (!File.Exists(ISMSSwaggerFilePath))
                    {
                        using (var sw = new StreamWriter(ISMSSwaggerFilePath))
                        {
                            sw.Write(AppResource.IdentityServer4_MicroService1);
                        }
                    }

                    c.IncludeXmlComments(ISMSSwaggerFilePath);
                });
            }
            #endregion

            #region Localization
            if (Options.Localization)
            {
                builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                services.Configure<RequestLocalizationOptions>(options =>
                {
                    var supportedCultures = new[]
                    {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN"),
                };
                    options.DefaultRequestCulture = new RequestCulture("zh-CN", "zh-CN");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

                builder.Services.AddMvc()
                    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(o =>
                    {
                        o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
            }
            #endregion

            #region ApiVersioning
            if (Options.ApiVersioning)
            {
                builder.Services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");

                builder.Services.AddApiVersioning(o =>
                {
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.ReportApiVersions = true;
                });
            }
            #endregion

            #region Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(AppConstant.AppAuthenScheme, isAuth =>
            {
                isAuth.Authority = configuration["IdentityServer"];
                isAuth.ApiName = Options.MicroServiceName;
                isAuth.RequireHttpsMetadata = true;
            })
            .AddIdentityServer4MicroServiceOAuths(configuration);
            #endregion

            #region ApiVersioning
            if (Options.EnableResponseCaching)
            {
                builder.Services.AddResponseCaching();
            }
            #endregion

            var DBConnectionString = configuration["ConnectionStrings:DataBaseConnection"];

            var DbContextOptions = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnectionString,
            opts => opts.MigrationsAssembly("IdentityServer4.MicroService")));

            builder
                .AddCoreService()
                .AddEmailService(configuration.GetSection("MessageSender:Email"))
                .AddSmsService(configuration.GetSection("MessageSender:Sms"));

            builder.AddTenantStore(DbContextOptions);

            builder.AddIdentityStore(DbContextOptions, Options.IdentityOptions);

            builder.AddSqlCacheStore(DBConnectionString);

            builder.AddIdentityServer(DbContextOptions, configuration);

            builder.Services.AddMemoryCache();

            return builder;
        }

        /// <summary>
        /// Configures EF implementation of TenantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddTenantStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions)
        {
            builder.Services.AddDbContext<TenantDbContext>(DbContextOptions);

            builder.Services.AddScoped<TenantDbContext>();

            return builder;
        }

        /// <summary>
        /// Configures EF implementation of IdentityStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <param name="identityOptions">The identity options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions, 
            Action<IdentityOptions> identityOptions = null)
        {
            builder.Services.AddDbContext<UserDbContext>(DbContextOptions);

            builder.Services.AddScoped<UserDbContext>();

            builder.Services.AddScoped<UserManager<AppUser>>();

            builder.Services.AddIdentity<AppUser, AppRole>(identityOptions)
                  .AddDefaultUI(UIFramework.Bootstrap4)
                  .AddEntityFrameworkStores<UserDbContext>()
                  .AddDefaultTokenProviders();

            return builder;
        }

        /// <summary>
        /// Configures SqlCache Service
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="connection">database connection string</param>
        /// <param name="schemaName">table schemaName</param>
        /// <param name="tableName">table name</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddSqlCacheStore(
           this IId4MsServiceBuilder builder,
           string connection,string schemaName= "dbo", string tableName= "AppCache")
        {
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connection;
                options.SchemaName = schemaName;
                options.TableName = tableName;
            });

            return builder;
        }


        /// <summary>
        /// Configures EF implementation of TenantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions,IConfiguration configuration)
        {
            builder.Services.AddIdentityServer(config =>
            {
                // keep same Issuer for banlancer
                 config.IssuerUri = configuration["IdentityServer"];
                // config.PublicOrigin = "";
                // config.Discovery.CustomEntries.Add("custom_endpoint", "~/api/custom");
            })
             .AddSigningCredential(GetSigningCredential(configuration))
             .AddCustomAuthorizeRequestValidator<TenantAuthorizeRequestValidator>()
             .AddCustomTokenRequestValidator<TenantTokenRequestValidator>()
             .AddConfigurationStore(x => x.ConfigureDbContext = DbContextOptions)
             .AddOperationalStore(x => x.ConfigureDbContext = DbContextOptions)
             .AddExtensionGrantValidator<MobileCodeGrantValidator>()
             .AddAspNetIdentity<AppUser>();

            return builder;
        }

        static X509Certificate2 GetSigningCredential(IConfiguration Configuration)
        {
            if (!string.IsNullOrWhiteSpace(Configuration["IdentityServerCertificate"]) &&
                !string.IsNullOrWhiteSpace(Configuration["IdentityServerCertificate:CertPath"]) &&
                !string.IsNullOrWhiteSpace(Configuration["IdentityServerCertificate:CertPassword"]))
            {
                var CertPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                     Configuration["IdentityServerCertificate:CertPath"]);

                var CertPassword = Configuration["IdentityServerCertificate:CertPassword"];

                return new X509Certificate2(CertPath, CertPassword,
               X509KeyStorageFlags.MachineKeySet |
               X509KeyStorageFlags.PersistKeySet |
               X509KeyStorageFlags.Exportable);
            }

            return new X509Certificate2(AppResource.identityserver4_microservice, "214480728730881",
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet |
                X509KeyStorageFlags.Exportable);
        }
    }
}
