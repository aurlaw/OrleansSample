using System;
using System.Net;
using System.Threading.Tasks;
using OrleansSample.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Configuration;
using OrleansSample.SiloHost.Config;
using System.IO;

namespace OrleansSample.SiloHost
{
    class Program
    {

        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }
        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }
        private static async Task<ISiloHost> StartSilo()
        {

            var appOptions = Utilites.GetApplicationConfiguration(Path.Combine(AppContext.BaseDirectory));
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .AddAdoNetGrainStorage("OrleansStorage", options =>
                {
                    options.Invariant = appOptions.OrleansInvariant;
                    options.ConnectionString = appOptions.OrleansConnectionString;
                    options.UseJsonFormat = true;
                })
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

    }
}
