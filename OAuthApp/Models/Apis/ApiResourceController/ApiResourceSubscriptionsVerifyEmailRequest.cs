using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourceSubscriptionsVerifyEmailRequest
    {
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string email { get; set; }
    }
}
