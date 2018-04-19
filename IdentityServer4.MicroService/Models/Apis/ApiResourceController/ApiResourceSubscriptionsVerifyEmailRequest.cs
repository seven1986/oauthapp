using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceSubscriptionsVerifyEmailRequest
    {
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string email { get; set; }
    }
}
