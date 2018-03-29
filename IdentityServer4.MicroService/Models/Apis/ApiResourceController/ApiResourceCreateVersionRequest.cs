using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceCreateVersionRequest
    {
        /// <summary>
        /// 修订版 版本号
        /// </summary>
        public string revisionId { get; set; }

        /// <summary>
        /// 版本号（v2，v3）
        /// </summary>
        public string apiVersionName { get; set; }
    }
}
