namespace OAuthApp.Tenant
{
    public static class TenantDefaults
    {
        /// <summary>
        /// CacheKey
        /// </summary>
        public const string CacheKey = "TenantContext";

        /// <summary>
        /// ReflushDuration
        /// </summary>
        public const int ReflushDuration = 5;

        public const long DatabaseMaxSize = 1 * 1024 * 1024;

        public class Properties
        {
            /// <summary>
            /// 官方网站
            /// </summary>
            public const string WebSite = "WebSite";

            /// <summary>
            /// 开放平台
            /// </summary>
            public const string PortalSite = "PortalSite";

            /// <summary>
            /// 运营中心
            /// </summary>
            public const string AdminSite = "AdminSite";

            /// <summary>
            /// 关键字
            /// </summary>
            public const string Keywords = "Keywords";

            /// <summary>
            /// 描述
            /// </summary>
            public const string Summary = "Summary";

            /// <summary>
            /// 介绍
            /// </summary>
            public const string Description = "Description";

            /// <summary>
            /// 企业邮箱
            /// </summary>
            public const string EnterpriseEmail = "EnterpriseEmail";

            /// <summary>
            /// 站点统计代码
            /// </summary>
            public const string Tracking = "Tracking";

            /// <summary>
            /// 网站图标
            /// </summary>
            public const string Favicon = "Favicon";


            public const string SSOSignInDomain = "SSOSignInDomain";
        }
    }
}
