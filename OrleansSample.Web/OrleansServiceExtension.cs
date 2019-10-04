using OrleansSample.Interfaces;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using System.Linq;
using OrleansSample.Utilites.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Orleans.Hosting;

namespace OrleansSample.Web
{
    public static class OrleansServiceExtension
    {
        public static void AddOrleans(this IServiceCollection services, IConfiguration configuration)
        {

            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ApplicationOptions>(configuration);
            var appOptions = new ApplicationOptions();
            configuration.Bind(appOptions);

            var client = CreateOrleansClient(appOptions);
            services.AddSingleton<IClusterClient>(client);
            // var myOptions = new MyOptions();
            //Configuration.GetSection("Application").Bind(myOptions);


        }
        private static IClusterClient CreateOrleansClient(ApplicationOptions appOptions)
        {
            var clientBuilder = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = appOptions.ClusterId;
                    options.ServiceId = appOptions.ServiceId;
                })
                .AddSimpleMessageStreamProvider(Constants.StreamProvider)
                .ConfigureLogging(logging => logging.AddConsole());

            var client = clientBuilder.Build();
            /*
            an easier way to achieve client retries is to pass a retry delegate to the Connect() method. 
            The following is a simple example of how a fixed-interval retry could be implemented, 
            but such a delegate makes it easy to implement more advanced mechanisms such as exponential backoff. 
            */
            client.Connect(async ex => 
            {
                Console.WriteLine(ex);
                Console.WriteLine("Retrying orleans connection...");
                await Task.Delay(3000);
                return true;
            }).Wait();

            return client;
        }
    }
}
