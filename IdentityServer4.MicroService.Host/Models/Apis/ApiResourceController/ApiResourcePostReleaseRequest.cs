using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourcePostReleaseRequest
    {
        /// <summary>
        /// api的id 
        /// </summary>
        [Required]
        public string aid { get; set; }

        /// <summary>
        /// 更新内容
        /// </summary>
        [Required]
        public string notes { get; set; }
    }
}
