using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourcePublishRevisionsRequest
    {
        /// <summary>
        /// Api的ID
        /// </summary>
        [Required(ErrorMessage = "请填写ID")]
        public long id { get; set; }

        /// <summary>
        /// swagger文档Uri
        /// </summary>
        [Url(ErrorMessage = "文档路径格式错误")]
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 修订内容
        /// </summary>
        public string releaseNote { get; set; }
    }
}
