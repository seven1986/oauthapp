using Microsoft.WindowsAzure.Storage.Table;

namespace IdentityServer4.MicroService.AzureJobs.Models
{
    public class ApiResourceSubscriptionEntity : TableEntity
    {
        public ApiResourceSubscriptionEntity() { }
        public ApiResourceSubscriptionEntity(string apiResourceId, string email)
        {
            PartitionKey = apiResourceId;
            RowKey = email;
        }
    }
}
