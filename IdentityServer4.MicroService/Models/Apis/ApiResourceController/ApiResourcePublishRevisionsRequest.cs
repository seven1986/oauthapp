using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourcePublishRevisionsRequest
    {
        /// <summary>
        /// Api的ID
        /// </summary>
        [Required(ErrorMessage = "请填写Api的ID")]
        public string apiId { get; set; }

        /// <summary>
        /// swagger文档Uri
        /// </summary>
        [Url(ErrorMessage = "文档路径格式错误")]
        public string swaggerUrl { get; set; }

        /// <summary>
        /// 修订备注
        /// </summary>
        public string releaseNote { get; set; }
    }
}
