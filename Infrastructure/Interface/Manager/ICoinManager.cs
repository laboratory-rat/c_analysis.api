using Infrastructure.Model.Coin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface ICoinManager
    {
        Task<List<CoinShortDisplayModel>> GetAll();
    }
}
