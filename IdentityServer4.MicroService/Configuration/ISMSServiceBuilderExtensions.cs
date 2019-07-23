using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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
        /// <param name="ismsOptions">The Options.</param>
        /// <param name="configuration">The Configuration.</param>
        /// <returns></returns>
        public static IISMSServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IdentityServer4MicroServiceOptions> ismsOptions = null)
        {
            var Options = new IdentityServer4MicroServiceOptions();

            if (ismsOptions != null)
            {
                ismsOptions.Invoke(Options);
            }

            var builder = new ISMSServiceBuilder(services);
            builder.Services.AddSingleton(Options);

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
                                $"{AppConstant.MicroServiceName}.all"
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
                                $"{AppConstant.MicroServiceName}.all"
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
                            AuthorizationUrl = Options.IdentityServerUri.OriginalString + "/connect/authorize",
                            TokenUrl = Options.IdentityServerUri.OriginalString + "/connect/token",
                            Description = "勾选授权范围，获取Token",
                            Scopes = new Dictionary<string, string>(){
                            { "openid","用户标识" },
                            { "profile","用户资料" },
                            { AppConstant.MicroServiceName+ ".all","所有接口权限"},
                            }
                        });

                    var provider = services.BuildServiceProvider()
                                   .GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        c.SwaggerDoc(description.GroupName, new Info
                        {
                            Title = AppConstant.AssemblyName,
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

                    var SiteSwaggerFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, AppConstant.AssemblyName + ".xml");

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

                    if (Options.EnableISMSSwaggerGen)
                    {
                        c.IncludeXmlComments(ISMSSwaggerFilePath);
                    }
                });
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
                    .AddDataAnnotationsLocalization()
                    .AddJsonOptions(o =>
                    {
                        o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
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
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(AppConstant.AppAuthenScheme, isAuth =>
            {
                isAuth.Authority = configuration["IdentityServer:Host"];
                isAuth.ApiName = AppConstant.MicroServiceName;
                isAuth.RequireHttpsMetadata = true;
            })
            .AddIdentityServer4MicroServiceOAuths(configuration);
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
                DBConnectionString = configuration["ConnectionStrings:DataBaseConnection"];
            }
            catch
            {
                throw new KeyNotFoundException("appsettings.json文件，ConnectionStrings:DataBaseConnection");
            }

            var DbContextOptions = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnectionString,
            opts => opts.MigrationsAssembly("IdentityServer4.MicroService")));

            builder.AddCoreService();

            builder.AddEmailService(configuration.GetSection("IdentityServer:Email"));
         
            builder.AddSmsService(configuration.GetSection("IdentityServer:SMS"));

            builder.AddTenantStore(DbContextOptions);

            builder.AddIdentityStore(DbContextOptions, Options.AspNetCoreIdentityOptions);

            builder.AddSqlCacheStore(DBConnectionString);

            var certificate = GetSigningCredential(configuration);

            builder.AddIdentityServer(DbContextOptions, certificate);

            builder.Services.AddMemoryCache();

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

            }

            return new X509Certificate2(AppResource.identityserver4_microservice, "214480728730881",
                   X509KeyStorageFlags.MachineKeySet |
                   X509KeyStorageFlags.PersistKeySet |
                   X509KeyStorageFlags.Exportable);
        }
    }
}
