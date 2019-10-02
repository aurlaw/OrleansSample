using OrleansSample.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace OrleansSample.Grains
{
    [StorageProvider(ProviderName = Constants.StorageName)]
    public class HelloGrain : Orleans.Grain, IHello, IGrainMarker
    {
        private readonly ILogger logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        public Task<string> SayHello(string greeting)
        {
            logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }
    }
}
