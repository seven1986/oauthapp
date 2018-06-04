using IdentityServer4.MicroService.AppSettings;
using IdentityServer4.MicroService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

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
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityServer4MicroService(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var builder = services.AddIdentityServer4MicroServiceBuilder();

            //注册Lazy
            builder.Services.TryAddTransient(typeof(Lazy<>));

            builder.Services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));

            builder
                .AddAppUserMD5PasswordHasher()
                .AddRedisService()
                .AddTenantService()
                .AddSwaggerCodeGenService()
                .AddAzureStorageService()
                .AddApiLoggerService()
                .AddAuthorization()
                .AddEmailService(configuration.GetSection("MessageSender:Email"))
                .AddSmsService(configuration.GetSection("MessageSender:Sms"));

            return builder;
        }
    }
}
