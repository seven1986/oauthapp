using IdentityServer4.MicroService.Attributes;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
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
        public static IId4MsServiceBuilder AddAppUserMD5PasswordHasher(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddScoped<IPasswordHasher<AppUser>, IdentityMD5PasswordHasher>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddApiLoggerService(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddScoped<ApiLoggerService>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddRedisService(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddSingleton<RedisService>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddTenantService(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddSingleton<TenantService>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddSwaggerCodeGenService(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddSingleton<SwaggerCodeGenService>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddAzureStorageService(this IId4MsServiceBuilder builder)
        {
            builder.Services.AddSingleton<AzureStorageService>();

            return builder;
        }

        /// <summary>
        /// Adds the required platform services.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddAuthorization(this IId4MsServiceBuilder builder)
        {
           builder.Services.AddAuthorization(options =>
            {
                #region Client的权限策略
                var scopes = typeof(ClientScopes).GetFields();

                foreach (var scope in scopes)
                {
                    var scopeName = scope.GetRawConstantValue().ToString();

                    var scopeValues = scope.GetCustomAttribute<PolicyClaimValuesAttribute>().ClaimsValues;

                    options.AddPolicy(scopeName, policy => policy.RequireClaim(ClaimTypes.ClientScope, scopeValues));
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
