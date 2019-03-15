using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Attributes;
using IdentityServer4.MicroService.Configuration;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
using static IdentityServer4.MicroService.MicroserviceConfig;

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

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="ids4msOptions">The Options.</param>
        /// <param name="configuration">The Configuration.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services,
            IdentityServer4MicroServiceOptions ids4msOptions,
            IConfiguration configuration)
        {
            var builder = services.AddIdentityServer4MicroServiceBuilder();

            if (string.IsNullOrWhiteSpace(ids4msOptions.MicroServiceName))
            {
                ids4msOptions.MicroServiceName = "ids4.ms";
            }

            builder.Services.AddSingleton(ids4msOptions);

            #region Cors
            if (ids4msOptions.Cors)
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
            if (ids4msOptions.WebEncoders)
            {
                services.AddWebEncoders(opt =>
                {
                    opt.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });
            }
            #endregion

            #region AuthorizationPolicy
            if (ids4msOptions.AuthorizationPolicy)
            {
                builder.Services.AddAuthorization(options =>
                {
                    #region Client的权限策略
                    var scopes = typeof(ClientScopes).GetFields();

                    foreach (var scope in scopes)
                    {
                        var scopeName = scope.GetRawConstantValue().ToString();

                        var scopeItem = scope.GetCustomAttribute<PolicyClaimValuesAttribute>();

                        var scopeValues = scopeItem.PolicyValues;

                        var scopeValuesList = new List<string>();

                        for (var i = 0; i < scopeValues.Length; i++)
                        {
                            scopeValues[i] = ids4msOptions.MicroServiceName + "." + scopeValues[i];

                            scopeValuesList.Add(scopeValues[i]);
                        }

                        scopeValuesList.Add(ids4msOptions.MicroServiceName + "." + scopeItem.ControllerName + ".all");

                        scopeValuesList.Add(ids4msOptions.MicroServiceName + ".all");

                        options.AddPolicy(scopeName, policy => policy.RequireClaim(ClaimTypes.ClientScope, scopeValuesList));
                    }
                    #endregion

                    #region User的权限策略
                    var permissions = typeof(UserPermissions).GetFields();

                    foreach (var permission in permissions)
                    {
                        var permissionName = permission.GetRawConstantValue().ToString();

                        var permissionItem = permission.GetCustomAttribute<PolicyClaimValuesAttribute>();

                        var permissionValues = permissionItem.PolicyValues;

                        var permissionValuesList = new List<string>();

                        for (var i = 0; i < permissionValues.Length; i++)
                        {
                            permissionValues[i] = ids4msOptions.MicroServiceName + "." + permissionValues[i];

                            permissionValuesList.Add(permissionValues[i]);
                        }

                        permissionValuesList.Add(ids4msOptions.MicroServiceName + "." + permissionItem.ControllerName + ".all");

                        permissionValuesList.Add(ids4msOptions.MicroServiceName + ".all");

                        options.AddPolicy(permissionName,
                            policy => policy.RequireAssertion(context =>
                            {
                                var userPermissionClaim = context.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.UserPermission));

                                if (userPermissionClaim != null && !string.IsNullOrWhiteSpace(userPermissionClaim.Value))
                                {
                                    var userPermissionClaimValue = userPermissionClaim.Value.ToLower().Split(new string[] { "," },
                                        StringSplitOptions.RemoveEmptyEntries);

                                    if (userPermissionClaimValue != null && userPermissionClaimValue.Length > 0)
                                    {
                                        foreach (var userPermissionItem in userPermissionClaimValue)
                                        {
                                            if (permissionValuesList.Contains(userPermissionItem))
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
            }
            #endregion

            #region SwaggerGen
            if (ids4msOptions.SwaggerGen)
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
                            AuthorizationUrl = ids4msOptions.IdentityServer.ToString() + "/connect/authorize",
                            TokenUrl = ids4msOptions.IdentityServer.ToString() + "/connect/token",
                            Description = "勾选授权范围，获取Token",
                            Scopes = new Dictionary<string, string>(){
                            { "openid","用户标识" },
                            { "profile","用户资料" },
                            { ids4msOptions.MicroServiceName+ ".all","所有接口权限"},
                            }
                        });

                    var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerDoc(description.GroupName, new Info
                        {
                            Title = ids4msOptions.AssemblyName,
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

                    var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, ids4msOptions.AssemblyName + ".xml");

                    c.IncludeXmlComments(filePath);

                    c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "IdentityServer4.MicroService.xml"));
                });
            }
            #endregion

            #region Localization
            if (ids4msOptions.Localization)
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
            if (ids4msOptions.ApiVersioning)
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
            if (ids4msOptions.IdentityServer != null)
            {
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(AppConstant.AppAuthenScheme, isAuth =>
                {
                    isAuth.Authority = "https://" + ids4msOptions.IdentityServer.Host;
                    isAuth.ApiName = ids4msOptions.MicroServiceName;
                    isAuth.RequireHttpsMetadata = true;
                })
                .AddIdentityServer4MicroServiceOAuths(configuration);
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

            builder.AddIdentityStore(DbContextOptions);

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
             .AddAspNetIdentity<AppUser>();

            return builder;
        }

       static X509Certificate2 GetSigningCredential(IConfiguration Configuration)
        {
            var certFilePath = Configuration["IdentityServerCertificate:FilePath"];

            var certPassword = Configuration["IdentityServerCertificate:CertPassword"];

            if (!string.IsNullOrWhiteSpace(certFilePath) &&
                !string.IsNullOrWhiteSpace(certPassword))
            {
                var certPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, certFilePath);

                var cert = new X509Certificate2(certPath, certPassword,
                    X509KeyStorageFlags.MachineKeySet |
                    X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.Exportable);

                return cert;
            }

            return null;
        }
    }
}
