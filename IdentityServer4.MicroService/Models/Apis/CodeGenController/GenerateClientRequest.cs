using System.ComponentModel;
using System.Runtime.Serialization;

namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
    public class GenerateClientRequest
    {
        /// <summary>
        /// 包平台
        /// </summary>
       public PackagePlatform platform { get; set; }

        /// <summary>
        /// 模板标识
        /// </summary>
       public string templateKey { get; set; }

        /// <summary>
        /// 模板是生成设置
        /// </summary>
       public string packageOptions { get; set; }
    }

    public enum PackagePlatform
    {
        /// <summary>
        /// Js包平台：NPM，网址：https://www.npmjs.com/
        /// </summary>
        [EnumMember(Value = "npm")]
        npm = 0,

        /// <summary>
        /// C#包平台：nuget，网址：https://www.nuget.org/
        /// </summary>
        [EnumMember(Value = "nuget")]
        nuget = 1,

        /// <summary>
        /// Android包平台：gradle，网址：https://gradle.org/ 
        /// </summary>
        [EnumMember(Value = "gradle")]
        gradle = 2,

        /// <summary>
        /// IOS包平台：SPM,网址：https://swift.org/package-manager/ 
        /// </summary>
        [EnumMember(Value = "spm")]
        spm = 3,
    }
}
