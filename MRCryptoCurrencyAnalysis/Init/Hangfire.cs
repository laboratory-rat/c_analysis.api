using Hangfire;
using Hangfire.Mongo;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MRCryptocompareClient.Client;
using MRMongoTools.Infrastructure.Settings;
using Service;

namespace MRCryptoCurrencyAnalysis.Init
{
    public static class Hangfire
    {
        public static IServiceCollection InitHandfire(this IServiceCollection services, IConfiguration configuration, MRDatabaseConnectionSettings connectionSettings)
        {
            services.AddHangfire(cfg =>
            {
                cfg.UseMongoStorage(connectionSettings.ConnectionString, connectionSettings.Database);
                cfg.UseNLogLogProvider();
            });

            GlobalConfiguration.Configuration.UseMongoStorage(connectionSettings.ConnectionString, connectionSettings.Database);
            GlobalConfiguration.Configuration.UseNLogLogProvider();

            RecurringJob.AddOrUpdate<CoinHistoryService>(nameof(CoinHistoryService), service => service.Action(), Cron.Hourly);

            return services;
        }
    }
}
