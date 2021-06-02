using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Cosmos.Table;

namespace OAuthApp.Services
{
    public class AzureStorageService
    {
        string Connection { get; set; }

        public AzureStorageService(
            IConfiguration configuration)
        {
            Connection = configuration.GetConnectionString("AzureStorageConnection");
        }

        public async Task<BlobContainerClient> CreateBlobAsync(string blobContainerName)
        {
            var blobClient = new BlobServiceClient(Connection);
            
            var blobContainer = blobClient.GetBlobContainerClient(blobContainerName);

            await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            return blobContainer;
        }
        public async Task<string> UploadBlobAsync(Stream stream, string blobContainerName, string blobName)
        {
            if (stream == null || string.IsNullOrWhiteSpace(blobContainerName)) { return string.Empty; }

            try
            {
                var blobContainer = await CreateBlobAsync(blobContainerName);
                
                var blockBlob =  blobContainer.UploadBlob(blobName, stream);

                return blobContainer.Uri.ToString() + "/" + blobName;
            }
            catch
            {
                throw;
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
            catch
            {
                throw;
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
            // Create the queue client.
            var queueClient = new QueueServiceClient(Connection);

            // Retrieve a reference to a container.
            var queue = queueClient.GetQueueClient(queueName);

            // Create the queue if it doesn't already exist
           await queue.CreateIfNotExistsAsync();

           await queue.SendMessageAsync(message);

           return true;
        }
    }
}
