using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.AppTenantModels
{
    public class AppTenantQuery
    {
        /// <summary>
        /// 站点域名
        /// </summary>
        public string Host { get; set; }
    }

    public class AppTenantPublicModel
    {
        public long id { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 声明
        /// </summary>
        public Dictionary<string,string> claims { get; set; }
    }

    public class AppTenantPrivateModel
    {
        /// <summary>
        /// 租户身份认证Server地址
        /// </summary>
        public string IdentityServerIssuerUri { get; set; }

        /// <summary>
        /// 属性
        /// </summary>
        public Dictionary<string, string> properties { get; set; }
    }
}
