using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ISMSServiceBuilderExtensionsCore
    {
        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IISMSServiceBuilder AddCoreService(this IISMSServiceBuilder builder)
        {
            builder.Services.AddScoped<IPasswordHasher<AppUser>, IdentityMD5PasswordHasher>();
            builder.Services.AddSingleton<TenantService>();
            builder.Services.AddSingleton<RedisService>();
            builder.Services.AddSingleton<SwaggerCodeGenService>();
            builder.Services.AddSingleton<AzureStorageService>();
            //builder.Services.AddScoped<ApiLoggerService>();
            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static IISMSServiceBuilder AddEmailService(this IISMSServiceBuilder builder, IConfigurationSection config)
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
        public static IISMSServiceBuilder AddSmsService(this IISMSServiceBuilder builder, IConfigurationSection config)
        {
            builder.Services.Configure<SmsSenderOptions>(config);
            builder.Services.AddTransient<ISmsSender, SmsSender>();
            return builder;
        }

        /// <summary>
        /// Configures EF implementation of TenantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <returns></returns>
        public static IISMSServiceBuilder AddTenantStore(
            this IISMSServiceBuilder builder,
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
        public static IISMSServiceBuilder AddIdentityStore(
            this IISMSServiceBuilder builder,
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
        public static IISMSServiceBuilder AddSqlCacheStore(
           this IISMSServiceBuilder builder,
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
        /// Configures EF implementation of TenantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="DbContextOptions">The store options action.</param>
        /// <param name="certificate">The certificate.</param>
        /// <returns></returns>
        public static IISMSServiceBuilder AddIdentityServer(
            this IISMSServiceBuilder builder,
            Action<DbContextOptionsBuilder> DbContextOptions, X509Certificate2 certificate)
        {
            builder.Services.AddIdentityServer()
             .AddSigningCredential(certificate)
             .AddCustomAuthorizeRequestValidator<TenantAuthorizeRequestValidator>()
             .AddCustomTokenRequestValidator<TenantTokenRequestValidator>()
             .AddConfigurationStore(x => x.ConfigureDbContext = DbContextOptions)
             .AddOperationalStore(x => x.ConfigureDbContext = DbContextOptions)
             .AddExtensionGrantValidator<MobileCodeGrantValidator>()
             .AddAspNetIdentity<AppUser>();

            return builder;
        }
    }
}
