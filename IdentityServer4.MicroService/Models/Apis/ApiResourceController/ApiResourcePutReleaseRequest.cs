using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourcePutReleaseRequest
    {
        [Required]
        public string notes { get; set; }
    }
}
