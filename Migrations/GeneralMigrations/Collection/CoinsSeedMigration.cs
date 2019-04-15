using AutoMapper;
using Infrastructure.Entity.Coins;
using Infrastructure.Interface.Repository;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRCryptocompareClient.Client;
using MRMigrationMaster;
using MRMigrationMaster.Infrastructure.Attr;
using MRMigrationMaster.Infrastructure.Component;
using MRMigrationMaster.Infrastructure.Enum;
using MRMigrationMaster.Infrastructure.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralMigrations.Collection
{
    [MigrationAttr("Coins seed", "06.04.2019")]
    class CoinsSeedMigration : Migration, IMigration
    {
        public override async Task Action()
        {
            var provider = _services.BuildServiceProvider();

            var coinRepository = provider.GetRequiredService<ICoinRepository>();
            var client = provider.GetRequiredService<CryptocompareClient>();
            var mapper = provider.GetRequiredService<IMapper>();
            var settings = provider.GetRequiredService<CryptoSyncSettings>();

            var allCoins = await client.General.GetAllCoins();
            if (!allCoins.IsSuccess)
            {
                Log(allCoins.Message, LogType.DANGER);
                return;
            }

            int seeded = 0;
            for (var primaryIndex = 0; primaryIndex < settings.Primary.Count; primaryIndex++)
            {
                var primary = settings.Primary[primaryIndex];

                if (!allCoins.Data.Any(x => x.Value.Symbol == primary))
                    continue;

                var fromService = allCoins.Data[primary];
                if (fromService == null)
                    continue;

                var exists = await coinRepository.GetFirst(x => x.Symbol == fromService.Symbol);
                if (exists != null)
                {
                    exists.SortOrder = seeded;
                    await coinRepository.Replace(exists);

                    Log($"-- update {exists.FullName}", LogType.INFO);
                }
                else
                {
                    exists = mapper.Map<Coin>(fromService);
                    exists.SortOrder = seeded;
                    await coinRepository.Insert(exists);
                    Log($"-- seed {exists.FullName}", LogType.SUCCESS);
                }

                seeded++;
            }

            Log($"seeded {seeded} coins", LogType.SUCCESS);
        }
    }
}
