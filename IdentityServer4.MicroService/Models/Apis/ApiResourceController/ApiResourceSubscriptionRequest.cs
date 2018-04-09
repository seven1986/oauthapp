using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceSubscriptionRequest
    {
        [EmailAddress(ErrorMessage ="邮箱格式错误")]
        public string Email { get; set; }
    }
}
