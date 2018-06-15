using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.Shared
{
    /// <summary>
    /// TenantPublicModel
    /// </summary>
    public class TenantPublicModel
    {
        /// <summary>
        /// id
        /// </summary>
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
