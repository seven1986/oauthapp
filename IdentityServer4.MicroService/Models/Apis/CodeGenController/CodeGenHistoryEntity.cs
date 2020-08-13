using Microsoft.Azure.Cosmos.Table;

namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
   public class CodeGenHistoryEntity : TableEntity
    {
        public CodeGenHistoryEntity() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiResourceId">API的ID</param>
        /// <param name="sdkName">sdk包的名称</param>
        public CodeGenHistoryEntity(string apiResourceId, string sdkName)
        {
            PartitionKey = apiResourceId;
            RowKey = sdkName;
        }

        /// <summary>
        /// swaggerUrl
        /// </summary>
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string tags { get; set; }

        /// <summary>
        /// 发布日志
        /// </summary>
        public string releaseDate { get; set; }

        /// <summary>
        ///  SDK语言
        /// </summary>
        public string language { get; set; }
    }
}
