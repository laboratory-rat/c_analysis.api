using Infrastructure.Entity.Coins;
using Infrastructure.Interface.Repository;
using MRMongoTools.Component;
using MRMongoTools.Infrastructure.Settings;

namespace Repository
{
    public class CoinHistoryRepository : MRRepository<CoinHistory>, ICoinHistoryRepository
    {
        public CoinHistoryRepository(MRDatabaseConnectionSettings settings) : base(settings) { }
    }
}
