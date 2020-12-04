using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.PackageController
{
    public class PreCompileRequest
    {
        [Required]
        [Url]
        public string swaggerUri { get; set; }

        [Required]
        [Url]
        public string scriptUri { get; set; }

        public string packageVersion { get; set; }
    }
}
