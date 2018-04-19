using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.AzureJobs.Services
{
    public class AzureStorageService
    {
        string Connection { get; set; }

        public AzureStorageService()
        {
            Connection = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
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
            var storageAccount = CloudStorageAccount.Parse(Connection);

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
            catch (StorageException ex)
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
            var storageAccount = CloudStorageAccount.Parse(Connection);

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
