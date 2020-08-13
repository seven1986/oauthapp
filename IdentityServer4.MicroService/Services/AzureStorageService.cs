using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Cosmos.Table;
using CloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount;

namespace IdentityServer4.MicroService.Services
{
    public class AzureStorageService
    {
        string Connection { get; set; }
        readonly ILogger<AzureStorageService> logger;

        public AzureStorageService(
            IConfiguration configuration,
            ILogger<AzureStorageService> _logger)
        {
            Connection = configuration["ConnectionStrings:AzureStorageConnection"];
            logger = _logger;
        }

        public async Task<CloudBlobContainer> CreateBlobAsync(string blobContainerName)
        {
            var storageAccount = CloudStorageAccount.Parse(Connection);

            var blobClient = storageAccount.CreateCloudBlobClient();

            var blobContainer = blobClient.GetContainerReference(blobContainerName);

            if (blobContainer.CreateIfNotExistsAsync().Result)
            {
                var blobPermission = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };

                await blobContainer.SetPermissionsAsync(blobPermission);
            }

            return blobContainer;
        }
        public async Task<string> UploadBlobAsync(Stream stream, string blobContainerName, string blobName)
        {
            if (stream == null || string.IsNullOrWhiteSpace(blobContainerName)) { return string.Empty; }
            
            try
            {
                var blobContainer = await CreateBlobAsync(blobContainerName);

                var blockBlob = blobContainer.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMdd") + "/" + blobName);

                await blockBlob.UploadFromStreamAsync(stream);

                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CloudTable> CreateTableAsync(string tableName)
        {
            var storageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(Connection);

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);

            await table.CreateIfNotExistsAsync();

            return table;
        }
        public async Task<IList<TableResult>> TableInsertAsync(CloudTable table,
           params ITableEntity[] entities)
        {
            try
            {
                var operation = new TableBatchOperation();

                foreach (var entity in entities)
                {
                    operation.Insert(entity);
                }

                return await table.ExecuteBatchAsync(operation);
            }
            catch (Microsoft.Azure.Cosmos.Table.StorageException ex)
            {
                throw ex;
            }
        }

        public async Task<IList<T>> ExecuteQueryAsync<T>(CloudTable table, TableQuery<T> query, 
            CancellationToken ct = default(CancellationToken), Action<IList<T>> onProgress = null) where T : ITableEntity, new()
        {

            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {

                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = seg.ContinuationToken;
                items.AddRange(seg);
                onProgress?.Invoke(items);

            } while (token != null && !ct.IsCancellationRequested);

            return items;
        }

        public async Task<bool> AddMessageAsync(string queueName, string message)
        {
            var storageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(Connection);

            // Create the queue client.
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            var queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
           await queue.CreateIfNotExistsAsync();

           await queue.AddMessageAsync(new CloudQueueMessage(message));

           return true;
        }
    }
}
