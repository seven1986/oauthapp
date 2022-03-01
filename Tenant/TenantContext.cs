using System.Collections.Generic;

namespace OAuthApp.Tenant
{
    public class TenantContext
    {
        public long Id { get; set; }

        /// <summary>
        /// 租户
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Issuer
        /// </summary>
        public string ClaimsIssuer { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// 声明
        /// </summary>
        public Dictionary<string, string> Claims { get; set; }

        /// <summary>
        /// 租户所有者ID
        /// </summary>
        public long OwnerId { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string LogoUri { get; set; }

        /// <summary>
        /// 登录地址
        /// </summary>
        public string WebHookSignInUri { get; set; }
    }
}
