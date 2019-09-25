using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OrleansSample.Utilites.Config
{
    public class AppConfiguration
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
        public static T GetConfiguration<T>(string outputPath, string section) where T: class
        {
            var configuration = Activator.CreateInstance<T>();
            var iConfig = GetIConfigurationRoot(outputPath);
            iConfig
                .GetSection(section)
                .Bind(configuration);
                return configuration;
        }
        public static ApplicationOptions GetApplicationConfiguration(string outputPath)
        {
            // var configuration = new ApplicationOptions();

            // var iConfig = GetIConfigurationRoot(outputPath);

            // iConfig
            //     .GetSection("Application")
            //     .Bind(configuration);

            // return configuration;
            return GetConfiguration<ApplicationOptions>(outputPath, "Application");
        }
    }
}
