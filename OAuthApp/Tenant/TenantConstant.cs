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
}
