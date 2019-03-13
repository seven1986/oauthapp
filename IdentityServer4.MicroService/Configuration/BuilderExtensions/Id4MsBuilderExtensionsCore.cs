using IdentityServer4.MicroService;
using IdentityServer4.MicroService.Attributes;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static IdentityServer4.MicroService.MicroserviceConfig;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Id4MsBuilderExtensionsCore
    {
        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddCoreService(this IId4MsServiceBuilder builder)
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
        public static IId4MsServiceBuilder AddEmailService(this IId4MsServiceBuilder builder, IConfigurationSection config)
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
        public static IId4MsServiceBuilder AddSmsService(this IId4MsServiceBuilder builder, IConfigurationSection config)
        {
            builder.Services.Configure<SmsSenderOptions>(config);
            builder.Services.AddTransient<ISmsSender, SmsSender>();
            return builder;
        }
    }
}
