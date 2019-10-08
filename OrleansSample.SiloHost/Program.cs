using System;
using System.Net;
using System.Threading.Tasks;
using OrleansSample.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Microsoft.Extensions.Configuration;
using OrleansSample.Utilites.Config;
using System.IO;
using OrleansSample.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace OrleansSample.SiloHost
{
    class Program
    {
        public static  IConfiguration Configuration;
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
            var baseDir = Path.Combine(AppContext.BaseDirectory);
            Configuration = AppConfiguration.GetIConfigurationRoot(baseDir);

            var appOptions = AppConfiguration.GetApplicationConfiguration(baseDir);
            var dashboardOptions = AppConfiguration.GetConfiguration<DashboardOptions>(baseDir, "Dashboard");
            // define the cluster configuration
            var builder = new SiloHostBuilder();
            switch(appOptions.StorageType) 
            {
                case StorageType.AzureTable:
                    builder.AddAzureTableGrainStorage(Constants.StorageName, options => 
                    {
                        options.ConnectionString = appOptions.OrleansConnectionString;
                        options.TableName = appOptions.AzureTableName;
                        options.UseJson = appOptions.UseJson;
                    });
                    builder.UseAzureTableReminderService(options => {
                        options.ConnectionString = appOptions.OrleansConnectionString;
                    });                    
                break;
                default:
                    builder.AddAdoNetGrainStorage(Constants.StorageName, options =>
                    {
                        options.Invariant = appOptions.AdoInvariant;
                        options.ConnectionString = appOptions.OrleansConnectionString;
                        options.UseJsonFormat = appOptions.UseJson;
                    });
                    builder.UseAdoNetReminderService(options => {
                        options.ConnectionString = appOptions.OrleansConnectionString;
                    });
                break;
            }
            builder.UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = appOptions.ClusterId;
                    options.ServiceId = appOptions.ServiceId;
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGrainMarker).Assembly).WithReferences())
                .AddSimpleMessageStreamProvider(Constants.StreamProvider)
                .AddMemoryGrainStorage(Constants.StreamStorage)
                .ConfigureServices((service) => {
                    service.Configure<ApplicationOptions>(Configuration);
                    service.AddOptions();
                    service.AddSingleton<ApplicationOptions>(appOptions);
                    DependencyInjectionHelper.IocContainerRegistration(service);
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard(options => {
                        options.Host = dashboardOptions.Host;
                        options.Port = dashboardOptions.Port;
                        options.HostSelf = dashboardOptions.HostSelf;
                        options.CounterUpdateIntervalMs = dashboardOptions.CounterUpdateIntervalMs;
                });
            var host = builder.Build();
            await host.StartAsync();
            return host;
        }

    }
}
