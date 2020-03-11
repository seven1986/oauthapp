namespace IdentityServer4.MicroService.CacheKeys
{
    internal class CodeGenControllerKeys
    {
        /// <summary>
        /// （缓存KEY）
        /// 发布Npm包的配置
        /// Key的格式：{ApiRrsourceId}
        /// Value的格式： json字符串
        /// </summary>
        public const string NpmOptions = "NpmOptions:";


        /// <summary>
        /// （缓存KEY）
        /// 发布Github的配置
        /// Key的格式：{ApiRrsourceId}
        /// Value的格式： json字符串
        /// </summary>
        public const string GithubOptions = "GithubOptions:";

        /// <summary>
        /// （缓存KEY）
        /// API的CodeGen的基本信息配置
        /// Key的格式：{ApiRrsourceId}
        /// Value的格式： json字符串
        /// </summary>
        public const string CommonOptions = "CommonOptions:";
    }
}
