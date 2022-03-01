using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.StorageController
{
    public class StorageTableResponse
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 总数据量
        /// </summary>
        public long RowsCount { get; set; }
    }
}
