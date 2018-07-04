using IdentityServer4.MicroService.AppSettings;
using IdentityServer4.MicroService.Configuration;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            var ConnectionSection = configuration.GetSection("ConnectionStrings");

            builder.Services.Configure<ConnectionStrings>(ConnectionSection);

            //注册Lazy
            builder.Services.TryAddTransient(typeof(Lazy<>));

            builder.Services.AddMemoryCache();

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

        /// <summary>
        /// Configures EF implementation of IPersistedGrantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddTenantStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> storeOptionsAction = null)
        {
            builder.Services.AddDbContext<TenantDbContext>(storeOptionsAction);
            builder.Services.AddScoped<TenantDbContext>();

            return builder;
        }

        /// <summary>
        /// Configures EF implementation of IPersistedGrantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <param name="identityOptions">The identity options action.</param>
        /// <returns></returns>
        public static IId4MsServiceBuilder AddIdentityStore(
            this IId4MsServiceBuilder builder,
            Action<DbContextOptionsBuilder> storeOptionsAction = null, Action<IdentityOptions> identityOptions = null)
        {
            builder.Services.AddDbContext<IdentityDbContext>(storeOptionsAction);
            builder.Services.AddScoped<IdentityDbContext>();

            builder.Services.AddIdentity<AppUser, AppRole>(identityOptions)
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            return builder;
        }
    }
}
