using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Options;

namespace IdentityServer4.MicroService.Services
{
    public class AzureStorageService
    {
        string Connection { get; set; }

        public AzureStorageService(IOptions<ConnectionStrings> _config)
        {
            Connection = _config.Value.AzureStorageConnection;
        }

        public async Task<CloudBlobContainer> CreateBlobContainerIfNotExists(string blobContainerName)
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

                var blobContainer = await CreateBlobContainerIfNotExists(blobContainerName);
                var blockBlob = blobContainer.GetBlockBlobReference(DateTime.UtcNow.ToString("yyyyMMdd") + "/" + blobName);

                blockBlob.UploadFromStreamAsync(stream).Wait();

                return blockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
