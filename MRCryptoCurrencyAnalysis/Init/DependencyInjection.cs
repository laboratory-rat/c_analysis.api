using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRMongoTools.Extensions.Identity.Settings;
using MRMongoTools.Infrastructure.Settings;
using MRMongoTools.Extensions.Identity.Service;
using AutoMapper;

namespace MRCryptoCurrencyAnalysis.Init
{
    public static class DependencyInjectionExtensions
    {
        public static void DependencyInjection(this IServiceCollection service, IConfiguration configuration)
        {
            MRTokenSettings tokenSettings = new MRTokenSettings();
            configuration.Bind("TokenSettings", tokenSettings);

            MRDatabaseConnectionSettings databaseSettings = new MRDatabaseConnectionSettings();
            configuration.Bind("DatabaseSettings", databaseSettings);

            service.AddMRMongoIdentity(databaseSettings, tokenSettings, action =>
            {

            });

            service.AddAutoMapper();
        }
    }
}
