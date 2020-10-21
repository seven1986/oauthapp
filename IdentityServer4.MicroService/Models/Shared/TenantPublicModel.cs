using System.Collections.Generic;

namespace OAuthApp.Models.Shared
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

        /// <summary>
        /// 简介
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string logoUri { get; set; }
    }
}
