namespace IdentityServer4.MicroService.Tenant
{
    public class TenantConstant
    {
        /// <summary>
        /// for request context
        /// </summary>
        public const string CacheKey = "AppTenant:";

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
