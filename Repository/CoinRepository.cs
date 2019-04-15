using Infrastructure.Entity.Coins;
using Infrastructure.Interface.Repository;
using MRMongoTools.Component;
using MRMongoTools.Infrastructure.Settings;

namespace Repository
{
    public class CoinRepository : MRRepository<Coin>, ICoinRepository
    {
        public CoinRepository(MRDatabaseConnectionSettings settings) : base(settings) { }
    }
}
