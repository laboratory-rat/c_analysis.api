using Infrastructure.Entity.Coins;
using MRMongoTools.Infrastructure.Interface;

namespace Infrastructure.Interface.Repository
{
    public interface ICoinRepository : IRepository<Coin>
    {
    }
}
