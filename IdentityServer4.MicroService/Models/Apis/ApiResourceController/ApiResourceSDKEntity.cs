using Microsoft.Azure.Cosmos.Table;

namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
    public class ApiResourceSDKEntity : TableEntity
    {
        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 发布者
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int ShowIndex { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 包平台
        /// npm/nuget/gradle/spm/postman
        /// </summary>
        public string PackagePlatform { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }

        public ApiResourceSDKEntity() { }

        public ApiResourceSDKEntity(string apiResourceId, string sdkName)
        {
            PartitionKey = apiResourceId;
            RowKey = sdkName;
        }
    }
}
