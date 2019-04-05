using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using MRMigrationMaster;
using System.IO;
using MRCryptoCurrencyAnalysis;

namespace GeneralMigrations
{
    public class GeneralMigrationsMaster : Master
    {
        public GeneralMigrationsMaster() : base() { }

        protected override IConfiguration ConfigurationInit()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        protected override IServiceCollection ServicesInit()
        {
            var startup = new Startup(_configuration);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            return services;
        }
    }
}
