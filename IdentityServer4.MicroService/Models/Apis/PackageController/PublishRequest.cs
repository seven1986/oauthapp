namespace OAuthApp.Models.Apis.PackageController
{
   public class PublishRequest
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags{ get; set; }

        /// <summary>
        /// 更新内容
        /// </summary>
        public string Description { get; set; }
    }
}
