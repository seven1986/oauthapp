using IdentityServer4.MicroService;
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
using System.Security.Cryptography.X509Certificates;

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
        /// <param name="configuration">The config.</param>
        /// <param name="assemblyName">The assemblyName.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services,
            IConfiguration configuration,
            string assemblyName)
        {
            var builder = services.AddIdentityServer4MicroServiceBuilder();

            #region cors-allowanonymous
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
            #endregion

            //var ConnectionSection = configuration.GetSection("ConnectionStrings");
            //builder.Services.Configure<ConnectionStrings>(ConnectionSection);

            #region Authentication & OAuth
            //Authentication 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            //IdentityServerAuthentication
            .AddIdentityServerAuthentication(AppConstant.AppAuthenScheme, isAuth =>
            {
                isAuth.Authority = "https://" + configuration["IdentityServer"];
                isAuth.ApiName = MicroserviceConfig.MicroServiceName;
                isAuth.RequireHttpsMetadata = true;
            })
            //OAuths Login
            .AddIdentityServer4MicroServiceOAuths(configuration);
            #endregion

            #region Mvc + localization
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
            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            services.AddMvc()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ReportApiVersions = true;
            });
            #endregion

            #region SwaggerGen
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
                        AuthorizationUrl = "https://" + configuration["IdentityServer"] + "/connect/authorize",
                        TokenUrl = "https://" + configuration["IdentityServer"] + "/connect/token",
                        Description = "勾选授权范围，获取Token",
                        Scopes = new Dictionary<string, string>(){
                            { "openid","用户标识" },
                            { "profile","用户资料" },
                            { MicroserviceConfig.MicroServiceName+ ".all","所有接口权限"},
                        }
                    });

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

                    c.CustomSchemaIds(x => x.FullName);
                }

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, assemblyName + ".xml");

                c.IncludeXmlComments(filePath);

                c.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "IdentityServer4.MicroService.xml"));
            });
            #endregion

            var DBConnectionString = configuration["ConnectionStrings:DataBaseConnection"];

            var DbContextOptions = new Action<DbContextOptionsBuilder>(x =>
            x.UseSqlServer(DBConnectionString,
            opts => opts.MigrationsAssembly("IdentityServer4.MicroService")));

            builder
                .AddCoreService()
                .AddAuthorization()
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
                // config.IssuerUri = "https://" + Configuration["IdentityServer"];
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
