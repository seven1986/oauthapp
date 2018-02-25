using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.Shared
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
        /// 官方网站
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// 开放平台
        /// </summary>
        public string PortalSite { get; set; }

        /// <summary>
        /// 运营中心
        /// </summary>
        public string AdminSite { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 企业邮箱
        /// </summary>
        public string EnterpriseEmail { get; set; }
    }
}
