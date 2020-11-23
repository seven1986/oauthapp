using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.CodeGenController
{
    public class CodeGenNpmOptionsModel
    {
        /// <summary>
        /// 包名称
        /// </summary>
        [Required(ErrorMessage = "请设置包名称")]
        public string name { get; set; }

        /// <summary>
        /// 包名称
        /// </summary>
        [Required(ErrorMessage = "调用时包的class名称")]
        public string sdkName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [Required(ErrorMessage = "请设置版本号")]
        public string version { get; set; }

        /// <summary>
        /// 主页
        /// </summary>
        [Required(ErrorMessage = "请设置主页")]
        public string homepage { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string[] keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// NPM发包Token
        /// </summary>
        [Required(ErrorMessage = "请设置发包Token")]
        public string token { get; set; }

        /// <summary>
        /// 协议
        /// </summary>
        public string license { get; set; } = "Unlicense";

        /// <summary>
        /// readme 文件的内容
        /// </summary>
        public string README { get; set; }
    }
}
