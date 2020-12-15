namespace OAuthApp.Tenant
{
    public class TenantConstant
    {
        /// <summary>
        /// for request context
        /// </summary>
        public const string CacheKey = "AppTenant:";

        /// <summary>
        /// for requst context in server
        /// </summary>
        public const string HttpContextItemKey = "AppTenant_Public";

        /// <summary>
        /// for Token 
        /// </summary>
        public const string TokenKey = "tenant";

        /// <summary>
        /// for reflush tenant schemes
        /// </summary>
        public const string SchemesReflush = "tenantSchemesReflush:";

        /// <summary>
        /// 
        /// </summary>
        public const int SchemesReflushDuration = 5;
    }

    public static class UserInteractionKeys
    {
        public const string prefix = "IdentityServer:";

        public const string Enable = prefix + ":EnableUserInteraction";

        public const string LoginUrl = prefix + "LoginUrl";

        public const string LoginReturnUrlParameter = prefix + "LoginReturnUrlParameter";

        public const string LogoutUrl = prefix + "LogoutUrl";

        public const string LogoutIdParameter = prefix + "LogoutIdParameter";

        public const string ConsentUrl = prefix + "ConsentUrl";

        public const string ConsentReturnUrlParameter = prefix + "ConsentReturnUrlParameter";

        public const string ErrorUrl = prefix + "ErrorUrl";

        public const string ErrorIdParameter = prefix + "ErrorIdParameter";

        public const string CustomRedirectReturnUrlParameter = prefix + "CustomRedirectReturnUrlParameter";

        public const string CookieMessageThreshold = prefix + "CookieMessageThreshold";

        public const string DeviceVerificationUrl = prefix + "DeviceVerificationUrl";

        public const string DeviceVerificationUserCodeParameter = prefix + "DeviceVerificationUserCodeParameter";
    }
}
