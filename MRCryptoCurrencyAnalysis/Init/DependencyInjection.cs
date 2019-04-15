using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Extensions;
using MRMongoTools.Extensions.Identity.Settings;
using MRMongoTools.Infrastructure.Settings;
using MRMongoTools.Extensions.Identity.Service;
using AutoMapper;
using Infrastructure.Settings;
using MRCryptocompareClient.Client;
using Infrastructure.Interface.Repository;
using Repository;
using Manager;
using Service;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using MRIdentityClient;
using Infrastructure.Entity.User;

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

            CryptoCompareSettings ccSettings = new CryptoCompareSettings();
            configuration.Bind("CryptoCompare", ccSettings);

            CryptoSyncSettings cryptoSyncSettings = new CryptoSyncSettings();
            configuration.Bind("CryptoSyncSettings", cryptoSyncSettings);
            service.AddSingleton(cryptoSyncSettings);

            service.AddMRMongoIdentity<UserEntity, UserRepository, IdentityUserManager>(databaseSettings, tokenSettings);

            var ccClient = new CryptocompareClient()
                .SetCredentions(ccSettings.Token, ccSettings.ApplicationName);
            service.AddSingleton(ccSettings);
            service.AddSingleton(ccClient);

            service.AddAutoMapper();

            LogManager.LoadConfiguration("NLog.config");

            // add identity client
            service.AddTransient((provider) => new IdentityClient("5QNCJF63Z81M28HY5WZVH70W7UDFQO0Q"));

            service.AddLogging(x =>
            {
                x.ClearProviders();
                x.AddNLog();
            });

            service.AddCors(opt =>
            {
                opt.AddPolicy("ALL", builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });

                opt.DefaultPolicyName = "ALL";
            });

            // scrum

            service.Scan((scan) =>
            {
                scan
                    .FromAssemblyOf<ICoinRepository>()
                        .AddClasses()
                        .AsImplementedInterfaces()
                    .FromAssemblyOf<CoinRepository>()
                        .AddClasses()
                        .AsMatchingInterface()
                    .FromAssemblyOf<CoinManager>()
                        .AddClasses()
                        .AsMatchingInterface()
                    .FromAssemblyOf<CoinHistoryService>()
                        .AddClasses()
                        .AsSelf();
            });

            service.InitHandfire(configuration, databaseSettings);
        }
    }
}
