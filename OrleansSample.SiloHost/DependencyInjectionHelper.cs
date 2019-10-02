using Microsoft.Extensions.DependencyInjection;
using OrleansSample.Grains;
using OrleansSample.Interfaces;

namespace OrleansSample.SiloHost
{
    public static  class DependencyInjectionHelper
    {
        /// <summary>
        /// Register concretions for DI.
        /// </summary>
        /// <param name="serviceCollection">The service collection in which to register thingers.</param>
        public static void IocContainerRegistration(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEmailSender, FakeEmailSender>();
        }        
    }
}