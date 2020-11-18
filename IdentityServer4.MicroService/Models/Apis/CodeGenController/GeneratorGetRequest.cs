namespace OAuthApp.Models.Apis.CodeGenController
{
   public class GeneratorGetRequest
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 是否展开所有Template（默认为false）
        /// </summary>
        public bool expandGenerators { get; set; } = false;
    }
}
