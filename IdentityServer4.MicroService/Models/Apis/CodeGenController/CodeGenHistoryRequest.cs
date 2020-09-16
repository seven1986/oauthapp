namespace OAuthApp.Models.Apis.CodeGenController
{
   public class CodeGenHistoryRequest
    {
        /// <summary>
        /// SDK名称
        /// </summary>
        public string sdkName { get; set; }

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
