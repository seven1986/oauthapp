using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.CodeGenController
{
    public class CodeGenGenRequest
    {
        /// <summary>
        /// 生成器名称。当前可选（readthedocs.gen）
        /// </summary>
        [Required(ErrorMessage = "请选择生成器")]
        public string genName { get; set; }

        /// <summary>
        /// swagger地址
        /// </summary>
        [Required(ErrorMessage = "请传入swagger地址")]
        public string swaggerUrl { get; set; }
    }

    
}
