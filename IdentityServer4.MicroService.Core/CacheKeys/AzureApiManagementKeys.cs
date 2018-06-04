namespace IdentityServer4.MicroService.CacheKeys
{
    public class AzureApiManagementKeys
    {
        /// <summary>
        /// Security>>Credentials.Management API URL
        /// </summary>
        public const string Host = "Azure:ApiManagement:Host";

        /// <summary>
        /// Security>>Credentials.Identifier
        /// </summary>
        public const string ApiId = "Azure:ApiManagement:ApiId";

        /// <summary>
        /// Security>>Credentials.Primary Key
        /// </summary>
        public const string ApiKey = "Azure:ApiManagement:ApiKey";

        /// <summary>
        /// Security>>OAuth 2.0 Authorization Servers 必须
        /// </summary>
        public const string AuthorizationServerId = "Azure:ApiManagement:AuthorizationServerId";

        /// <summary>
        /// 非必需配置
        /// </summary>
        public const string ProductId = "Azure:ApiManagement:ProductId";

        /// <summary>
        ///  Portal Host集合，无需加http开后，多个用逗号分隔
        /// </summary>
        public const string PortalUris = "Azure:ApiManagement:PortalUris";

        /// <summary>
        /// Security>>Delegation.Delegation Validation Key
        /// </summary>
        public const string DelegationKey = "Azure:ApiManagement:DelegationKey";
    }
}
