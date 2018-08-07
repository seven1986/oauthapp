using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
    public class GenerateClientRequest
    {
        /// <summary>
        /// 包平台
        /// </summary>
        [Required(ErrorMessage = "请选择发包平台")]
        public PackagePlatform platform { get; set; }

        /// <summary>
        /// 语言(可选：angular2/jQuery)
        /// </summary>
        [Required(ErrorMessage = "请选择模板语言")]
        public Language language { get; set; }

        /// <summary>
        /// 微服务的ID
        /// </summary>
        [Required(ErrorMessage = "请填写微服务的ID")]
        public string apiId { get; set; }

        /// <summary>
        /// swagger 文档地址
        /// </summary>
        [Required(ErrorMessage ="请填写swagger文档地址")]
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 自定义标签
        /// </summary>
        public string tags { get; set; }
    }

    public enum Language
    {
        /// <summary>
        /// angular2
        /// </summary>
        [EnumMember(Value = "angular2")]
        angular2 = 0,

        /// <summary>
        /// jquery
        /// </summary>
        [EnumMember(Value = "jQuery")]
        jQuery = 1,
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
