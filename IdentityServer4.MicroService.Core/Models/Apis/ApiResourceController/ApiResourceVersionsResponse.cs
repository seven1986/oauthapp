using System.Collections.Generic;
using IdentityServer4.MicroService.Services;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceVersionsResponse : AzureApiManagementApiEntity
    {
        public List<AzureApiManagementRevisionEntity> revisions { get; set; }
    }
}
