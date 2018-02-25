using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.Shared
{
    public class TenantPublicModel
    {
        public long id { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 声明
        /// </summary>
        public Dictionary<string, string> claims { get; set; }
    }
}
