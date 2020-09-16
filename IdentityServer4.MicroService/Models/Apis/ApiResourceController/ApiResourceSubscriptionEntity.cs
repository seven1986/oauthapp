using Microsoft.Azure.Cosmos.Table;

namespace OAuthApp.Models.Apis.ApiResourceController
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
