using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer4.MicroService.Models.Apis.ApiScopeController
{
   public class ApiScopeGetRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否展开所有Properties（默认为false）
        /// </summary>
        public bool expandProperties { get; set; } = false;

        /// <summary>
        /// 是否展开所有Claims（默认为false）
        /// </summary>
        public bool expandClaims { get; set; } = false;
    }
}
