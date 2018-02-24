using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.Shared
{
    public class TenantPrivateModel
    {
        public long id { get; set; }

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
