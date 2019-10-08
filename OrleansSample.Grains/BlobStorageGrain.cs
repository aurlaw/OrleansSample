using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using OrleansSample.Interfaces;
using OrleansSample.Interfaces.Service;

namespace OrleansSample.Grains
{
    public class BlobStorageGrain : Grain, IBlobStorageGrain, IGrainMarker
    {
        private IBlobStorage storage;
        private ILogger logger;

        public BlobStorageGrain(ILoggerFactory loggerFactory, IBlobStorage storage) 
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}-{this.IdentityString}");;
            this.storage = storage;
        }

        public async Task<string> UploadAsync(string name, string container, byte[] data)
        {
            logger.LogInformation($"Upload {name} to {container}");
            return await storage.Upload(name, container, data);
        }
    }
}