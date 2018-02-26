using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MicrosoftAccountExtensions2
    {
        public static AuthenticationBuilder AddMicrosoftAccount2(this AuthenticationBuilder builder)
            => builder.AddMicrosoftAccount2(MicrosoftAccountDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddMicrosoftAccount2(this AuthenticationBuilder builder, Action<MicrosoftAccountOptions> configureOptions)
            => builder.AddMicrosoftAccount2(MicrosoftAccountDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddMicrosoftAccount2(this AuthenticationBuilder builder, string authenticationScheme, Action<MicrosoftAccountOptions> configureOptions)
            => builder.AddMicrosoftAccount2(authenticationScheme, MicrosoftAccountDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddMicrosoftAccount2(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<MicrosoftAccountOptions> configureOptions)
            => builder.AddOAuth<MicrosoftAccountOptions, MicrosoftAccountHandler2>(authenticationScheme, displayName, configureOptions);
    }
}
