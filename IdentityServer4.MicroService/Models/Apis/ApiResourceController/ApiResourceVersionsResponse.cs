using System.Collections.Generic;
using OAuthApp.Services;

namespace OAuthApp.Models.Apis.ApiResourceController
{
    public class ApiResourceVersionsResponse : AzureApiManagementApiEntity
    {
        public List<AzureApiManagementRevisionEntity> revisions { get; set; }
    }
}
