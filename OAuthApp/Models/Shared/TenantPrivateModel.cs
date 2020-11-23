using System.Collections.Generic;

namespace OAuthApp.Models.Shared
{
    public class TenantPrivateModel
    {
        public long Id { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 租户身份认证Server地址
        /// </summary>
        public string IdentityServerIssuerUri { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// 租户所有者ID
        /// </summary>
        public long OwnerUserId { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string LogoUri { get; set; }
    }
}
