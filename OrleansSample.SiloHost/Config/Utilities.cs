using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OrleansSample.SiloHost.Config
{
    public class Utilites
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static ApplicationOptions GetApplicationConfiguration(string outputPath)
        {
            var configuration = new ApplicationOptions();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig
                .GetSection("Application")
                .Bind(configuration);

            return configuration;
        }
    }
}
