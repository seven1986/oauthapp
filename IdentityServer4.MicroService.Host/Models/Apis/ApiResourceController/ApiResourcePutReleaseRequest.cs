using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourcePutReleaseRequest
    {
        [Required]
        public string notes { get; set; }
    }
}
