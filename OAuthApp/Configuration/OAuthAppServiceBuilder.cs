using Microsoft.Extensions.DependencyInjection;
using System;

namespace OAuthApp.Configuration
{
   public class OAuthAppServiceBuilder : IOAuthAppServiceBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityServerBuilder"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <exception cref="System.ArgumentNullException">services</exception>
        public OAuthAppServiceBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>
        /// The services.
        /// </value>
        public IServiceCollection Services { get; }
    }
}
