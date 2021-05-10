using IdentityServer4.Configuration;
using OAuthApp;
using OAuthApp.Configuration;
using OAuthApp.Data;
using OAuthApp.Services;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.IdentityModel.Tokens;
using OAuthApp.Attributes;
using AspNetCoreRateLimit;
using IdentityServer4;
using System.IO.Compression;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OAuthAppServiceBuilderExtensions
    {
        private static IConfiguration Configuration { get; }

        static OAuthAppServiceBuilderExtensions()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true)
                .AddJsonFile("appsettings.Staging.json", true)
                .AddJsonFile("appsettings.Production.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="ismsOptions">The Options.</param>
        /// <returns></returns>
        public static IOAuthAppServiceBuilder AddOAuthApp(
            this IServiceCollection services,
            Action<OAuthAppOptions> ismsOptions = null)
        {
            var Options = new OAuthAppOptions();

            if (ismsOptions != null)
            {
                ismsOptions.Invoke(Options);
            }

            var builder = new OAuthAppServiceBuilder(services);
            builder.Services.AddSingleton(Options);

            builder.Services.AddHttpContextAccessor();

            #region Cors
            if (Options.EnableCors)
            {
                if (string.IsNullOrWhiteSpace(Options.Origins))
                {
                    try
                    {
                        Options.Origins = Configuration["IdentityServer:Origins"];
                    }
                    catch
                    {
                        throw;
                    }
                }

                if (!string.IsNullOrWhiteSpace(Options.Origins))
                {
                    var Origins = Options.Origins.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    builder.Services.AddCors(options =>
                    {
                        options.AddPolicy("cors-allowanonymous", x =>
                        {
                            x.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .WithOrigins(Origins);
                        });
                    });
                }
            }
            #endregion

            #region WebEncoders
            if (Options.EnableWebEncoders)
            {
                services.AddWebEncoders(opt =>
                {
                    opt.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });
            }
            #endregion

            #region AuthorizationPolicy
            if (Options.EnableAuthorizationPolicy)
            {
                builder.Services.AddAuthorization(options =>
                {
                    var ISMSTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => x.BaseType != null && x.BaseType.Name.Equals("ApiControllerBase")).ToList();

                    var isms_policies = PolicyConfigs(ISMSTypes);

                    var EntryTypes = Assembly.GetEntryAssembly().GetTypes()
                       .Where(x => x.BaseType != null &&
                       (x.BaseType.Name.Equals("ApiControllerBase") ||
                       x.BaseType.Name.Equals("ControllerBase"))).ToList();

                    var entry_policies = PolicyConfigs(EntryTypes);

                    if (entry_policies.Count > 0)
                    {
                        isms_policies.AddRange(entry_policies);
                    }

                    foreach (var policyConfig in isms_policies)
                    {
                        #region Client的权限策略
                        policyConfig.Scopes.ForEach(x =>
                        {
                            var policyName = $"{PolicyKey.ClientScope}:{x}";

                            var policyValues = new List<string>()
                            {
                                $"{AppConstant.MicroServiceName}.{x}",
                                $"{AppConstant.MicroServiceName}.{policyConfig.ControllerName}.all",
                                $"{AppConstant.MicroServiceName}.all",
                                "isms.all"
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
                                $"{AppConstant.MicroServiceName}.{x}",
                                $"{AppConstant.MicroServiceName}.{policyConfig.ControllerName}.all",
                                $"{AppConstant.MicroServiceName}.all",
                                "isms.all"
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
            if (Options.EnableSwaggerGen)
            {
                services.AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();

                    //c.TagActionsBy(x => x.RelativePath.Split('/')[0]);

                    //c.AddSecurityDefinition("SubscriptionKey",
                    //    new OpenApiSecurityScheme()
                    //    {
                    //        Name = "Ocp-Apim-Subscription-Key",
                    //        Type = SecuritySchemeType.ApiKey,
                    //        In = ParameterLocation.Header,
                    //        Description = "从开放平台申请的Subscription Key，从网关调用接口时必需传入。",
                    //    });

                    //c.AddSecurityDefinition("AccessToken",
                    //    new OpenApiSecurityScheme()
                    //    {
                    //        Name = "Authorization",
                    //        Type = SecuritySchemeType.OpenIdConnect,
                    //        In = ParameterLocation.Header,
                    //        Description = "从身份认证中心颁发的Token，根据接口要求决定是否传入。",
                    //    });

                    c.AddSecurityDefinition("OAuth2",
                        new OpenApiSecurityScheme()
                        {
                            Type = SecuritySchemeType.OAuth2,
                            Flows = new OpenApiOAuthFlows()
                            {
                                AuthorizationCode = new OpenApiOAuthFlow()
                                {
                                    AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                                    TokenUrl = new Uri("/connect/token", UriKind.Relative),
                                    Scopes = new Dictionary<string, string>(){
                                        { "openid","用户标识" },
                                        { "profile","用户资料" },
                                        { AppConstant.MicroServiceName+ ".all","所有接口权限"},
                                    },
                                }
                            },

                            Description = "勾选授权范围，获取access_token",
                        });

                    var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        var info = new OpenApiInfo
                        {
                            Title = AppConstant.AssemblyName,
                            Version = description.ApiVersion.ToString(),
                            License = new OpenApiLicense()
                            {
                                Name = "MIT",
                                Url = new Uri("https://spdx.org/licenses/MIT.html")
                            },
                            // Contact = new Contact()
                            // {
                            //     Url = "",
                            //     Name = "",
                            //     Email = ""
                            // },
                            // Description = "Swagger document",
                        };

                        if(Options.ReDocExtensions!=null)
                        {
                            Options.ReDocExtensions.Invoke(info.Extensions);
                        }

                        c.SwaggerDoc(description.GroupName, info);

                        c.OperationFilter<SwaggerUploadFileParametersFilter>();

                        //c.CustomSchemaIds(x => x.FullName);
                    }

                    var SiteSwaggerFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, AppConstant.AssemblyName + ".xml");

                    if (File.Exists(SiteSwaggerFilePath))
                    {
                        c.IncludeXmlComments(SiteSwaggerFilePath);
                    }

                    var ISMSSwaggerFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "OAuthApp.xml");

                    if (!File.Exists(ISMSSwaggerFilePath))
                    {
                        using var sw = new StreamWriter(ISMSSwaggerFilePath);
                        sw.Write(AppResource.OAuthApp);
                    }

                    if (Options.EnableAPIDocuments)
                    {
                        c.IncludeXmlComments(ISMSSwaggerFilePath);
                    }
                });

                services.AddSwaggerGenNewtonsoftSupport();
            }
            #endregion

            #region Localization
            if (Options.EnableLocalization)
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
                    .AddDataAnnotationsLocalization();
                //.AddJsonOptions(o =>
                //{
                //    //o.JsonSerializerOptions..ContractResolver = new CamelCasePropertyNamesContractResolver();
                //    //o.JsonSerializerOptions.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //});
            }
            #endregion

            #region ApiVersioning
            if (Options.EnableApiVersioning)
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
            builder.Services.AddAuthentication()
                .AddJwtBearer(AppConstant.AppAuthenScheme, options =>
            {
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new OAuthAppTokenValidation()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = AppConstant.MicroServiceName,
                    IssuerSigningKey = new X509SecurityKey(GetSigningCredential(Configuration))
                };
                
            }).AddOAuthPlatforms();
            #endregion

            #region ResponseCaching
            if (Options.EnableResponseCaching)
            {
                builder.Services.AddResponseCaching();
            }
            #endregion

            var DBConnectionString = string.Empty;

            try
            {
                DBConnectionString = Configuration["ConnectionStrings:DataBaseConnection"];
            }
            catch
            {
                throw new KeyNotFoundException("appsettings.json文件，ConnectionStrings:DataBaseConnection");
            }

            var DbContextOptions = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnectionString,
            opts => opts.MigrationsAssembly("OAuthApp")));

            builder.AddCoreService();

            builder.AddEmailService(Configuration.GetSection("IdentityServer:Email"));

            builder.AddSmsService(Configuration.GetSection("IdentityServer:SMS"));

            builder.AddSDKStore(DbContextOptions);

            builder.AddTenantStore(DbContextOptions);

            builder.AddIdentityStore(DbContextOptions, Options.AspNetCoreIdentityOptions);

            builder.AddSqlCacheStore(DBConnectionString);

            var certificate = GetSigningCredential(Configuration);

            builder.AddIdentityServer(DbContextOptions, certificate, Options.IdentityServerOptions, Options.IdentityServerBuilder);

            builder.Services.AddOptions();

            builder.Services.AddMemoryCache();

            builder.Services.AddMvc().AddNewtonsoftJson(options =>
            {
                //设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //数据格式按原样输出
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //忽略空值
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            
            var ClearScriptV8_64_FilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath+ "/runtimes/win-x64/native", "ClearScriptV8.win-x64.dll");
            if (!File.Exists(ClearScriptV8_64_FilePath))
            {
                using var sw = new StreamWriter(ClearScriptV8_64_FilePath);
                sw.Write(AppResource.ClearScriptV8_win_x64);
            }

            var ClearScriptV8_86_FilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath + "/runtimes/win-x86/native", "ClearScriptV8.win-x86.dll");
            if (!File.Exists(ClearScriptV8_86_FilePath))
            {
                using var sw = new StreamWriter(ClearScriptV8_86_FilePath);
                sw.Write(AppResource.ClearScriptV8_win_x86);
            }

            #region RateLimit
            if (Options.EnableClientRateLimit)
            {
                builder.AddClientRateLimit();
            }

            if (Options.EnableIpRateLimit)
            {
                builder.AddIpRateLimit();
            }

            if (Options.EnableClientRateLimit || Options.EnableIpRateLimit)
            {
                builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

                //https://github.com/stefanprodan/AspNetCoreRateLimit/issues/171
                //builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

                builder.Services.AddSingleton<IRateLimitConfiguration, OAuthAppRateLimitConfiguration>();
            }
            #endregion

            return builder;
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

        static X509Certificate2 GetSigningCredential(IConfiguration Configuration)
        {
            try
            {
                var certName = Configuration["IdentityServer:Certificate:FileName"];

                var certPWD = Configuration["IdentityServer:Certificate:CertPassword"];

                if (!string.IsNullOrWhiteSpace(certName) && !string.IsNullOrWhiteSpace(certPWD))
                {
                    var CertPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                         certName);

                    return new X509Certificate2(CertPath, certPWD,
                   X509KeyStorageFlags.MachineKeySet |
                   X509KeyStorageFlags.PersistKeySet |
                   X509KeyStorageFlags.Exportable);
                }
            }
            catch
            {
                throw;
            }

            return new X509Certificate2(AppResource.oauthapp1, "FPRa5vNO",
                   X509KeyStorageFlags.MachineKeySet |
                   X509KeyStorageFlags.PersistKeySet |
                   X509KeyStorageFlags.Exportable);
        }

        /// <summary>
        /// Adds ClientRateLimit.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddClientRateLimit(this IOAuthAppServiceBuilder builder)
        {
            builder.Services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            builder.Services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            builder.Services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();

            return builder;
        }

        /// <summary>
        /// Adds AddIpRateLimit.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddIpRateLimit(this IOAuthAppServiceBuilder builder)
        {
            builder.Services.Configure<IpRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));
            builder.Services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddCoreService(this IOAuthAppServiceBuilder builder)
        {
            builder.Services.AddScoped<IPasswordHasher<AppUser>, IdentityMD5PasswordHasher>();
            builder.Services.AddScoped<TenantService>();
            builder.Services.AddSingleton<RedisService>();
            builder.Services.AddSingleton<SwaggerCodeGenService>();
            builder.Services.AddSingleton<AzureStorageService>();
            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddEmailService(this IOAuthAppServiceBuilder builder, IConfigurationSection config)
        {
            builder.Services.Configure<EmailSenderOptions>(config);
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<EmailService>();
            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddSmsService(this IOAuthAppServiceBuilder builder, IConfigurationSection config)
        {
            builder.Services.Configure<SmsSenderOptions>(config);
            builder.Services.AddTransient<ISmsSender, SmsSender>();
            return builder;
        }

        /// <summary>
        /// Configures TenantStore.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddTenantStore(
            this IOAuthAppServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions)
        {
            builder.Services.AddDbContext<TenantDbContext>(DbContextOptions);
            return builder;
        }

        /// <summary>
        /// Configures IdentityStore.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <param name="identityOptions">The identity options action.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddIdentityStore(
            this IOAuthAppServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions,
            Action<IdentityOptions> identityOptions = null)
        {
            builder.Services.AddDbContext<UserDbContext>(DbContextOptions);
            builder.Services.AddScoped<UserDbContext>();
            builder.Services.AddScoped<UserManager<AppUser>>();

            builder.Services.AddIdentity<AppUser, AppRole>(identityOptions)
                  .AddDefaultUI()
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
        static IOAuthAppServiceBuilder AddSqlCacheStore(
           this IOAuthAppServiceBuilder builder,
           string connection, string schemaName = "dbo", string tableName = "AppCache")
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
        /// Configures CodeGenStore.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddSDKStore(
            this IOAuthAppServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions)
        {
            builder.Services.AddDbContext<SdkDbContext>(DbContextOptions);
            return builder;
        }


        /// <summary>
        /// Configures TenantStore.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="identityServerOptions">The identityServerOptions.</param>
        /// <param name="identityBuilder">The identityBuilder.</param>
        /// <returns></returns>
        static IOAuthAppServiceBuilder AddIdentityServer(
            this IOAuthAppServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions, X509Certificate2 certificate, Action<IdentityServerOptions> identityServerOptions = null, Action<IIdentityServerBuilder> identityBuilder = null)
        {
            IIdentityServerBuilder ISBuilder = null;

            if (identityServerOptions != null)
            {
                ISBuilder = builder.Services.AddIdentityServer(identityServerOptions);
            }
            else
            {
                ISBuilder = builder.Services.AddIdentityServer();
            }

            ISBuilder.AddSigningCredential(certificate)
              .AddCustomAuthorizeRequestValidator<TenantAuthorizeRequestValidator>()
              .AddCustomTokenRequestValidator<TenantTokenRequestValidator>()
              .AddConfigurationStore(x => x.ConfigureDbContext = DbContextOptions)
              .AddOperationalStore(x => x.ConfigureDbContext = DbContextOptions)
              .AddAspNetIdentity<AppUser>()
              .AddJwtBearerClientAuthentication()
              .AddAppAuthRedirectUriValidator()
              .AddRedirectUriValidator<AnonymousRedirectUriValidator>()
              .AddExtensionGrantValidator<MobileCodeGrantValidator>()
              .AddExtensionGrantValidator<OpenIdOAuthGrantValidator>();

            if (identityBuilder != null)
            {
                identityBuilder.Invoke(ISBuilder);
            }

            return builder;
        }
    }
}
