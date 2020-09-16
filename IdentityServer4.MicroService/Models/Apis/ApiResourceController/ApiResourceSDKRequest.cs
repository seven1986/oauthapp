using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourceSDKRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public string name { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 发布者
        /// </summary>
        [Required]
        public string Publisher { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Required]
        public string Link { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Required]
        public string Icon { get; set; }

        /// <summary>
        /// 包平台
        /// npm/nuget/gradle/spm
        /// </summary>
        [Required]
        public string PackagePlatform { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int ShowIndex { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }
    }
}
