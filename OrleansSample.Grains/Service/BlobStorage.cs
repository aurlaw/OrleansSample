using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrleansSample.Interfaces.Service;
using OrleansSample.Utilites.Config;

namespace OrleansSample.Grains.Service
{
    public class BlobStorage : IBlobStorage
    {
        private readonly ApplicationOptions options;
        private ILogger logger;

        public BlobStorage(ILoggerFactory loggerFactory, ApplicationOptions options)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}");
            this.options = options;
        }

        public async Task<string> Upload(string name, string container, byte[] data)
        {
            logger.LogInformation($"connection: {options.StorageConnection}");
            var storageAccount = CloudStorageAccount.Parse(options.StorageConnection);
            var client = storageAccount.CreateCloudBlobClient();
            // get/create container
            var blobContainer = client.GetContainerReference(container);
            await blobContainer.CreateIfNotExistsAsync();
            // Set the permissions so the blobs are public.
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await blobContainer.SetPermissionsAsync(permissions);
            // upload data
            var blockBlob = blobContainer.GetBlockBlobReference(name);
            await blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
            return blockBlob.Uri.AbsoluteUri;
        }
    }
}