using Microsoft.Azure.Cosmos.Table;

namespace IdentityServer4.MicroService.Models.Apis.ApiResourceController
{
    public class ApiResourceSubscriptionEntity: TableEntity
    {
        public ApiResourceSubscriptionEntity() { }
        public ApiResourceSubscriptionEntity(string apiResourceId,string email)
        {
            PartitionKey = apiResourceId;
            RowKey = email;
        }
    }
}
