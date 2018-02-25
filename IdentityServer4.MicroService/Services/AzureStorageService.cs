using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.MicroService.Services
{
    public class AzureStorageService
    {
        string Connection { get; set; }
        readonly ILogger<AzureStorageService> logger;

        public AzureStorageService(
            IOptions<ConnectionStrings> _config,
            ILogger<AzureStorageService> _logger)
        {
            Connection = _config.Value.AzureStorageConnection;
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
        public async Task<string> UploadBlob(Stream stream, string blobContainerName, string blobName)
        {
            if (stream == null || string.IsNullOrWhiteSpace(blobContainerName)) { return string.Empty; }
            
            try
            {
                if (blobName.Contains("wx-file"))
                {
                    blobName = string.Format("{0}-{1}", DateTime.UtcNow.ToString("HHmmssffff"), blobName);
                }

                var blobContainer = await CreateBlobAsync(blobContainerName);
                var blockBlob = blobContainer.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMdd") + "/" + blobName);

                blockBlob.UploadFromStreamAsync(stream).Wait();

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
        public async Task<IList<TableResult>> TableInsert(CloudTable table,
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
    }
}
