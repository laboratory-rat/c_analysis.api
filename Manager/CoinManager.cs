using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Entity.User;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.Coin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRMongoTools.Extensions.Identity.Interface;

namespace Manager
{
    public class CoinManager : BaseManager, ICoinManager
    {
        protected readonly ICoinRepository _coinRepository;

        public CoinManager(ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper, 
            IdentityUserManager mrUserManager, IMRUserStore<UserEntity> mrUserStore, ICoinRepository coinRepository) : base(loggerFactory, httpContextAccessor, mapper, mrUserManager, mrUserStore)
        {
            _coinRepository = coinRepository;
        }

        public async Task<List<CoinShortDisplayModel>> GetAll()
        {
            var entities = (await _coinRepository.GetSorted(x => x.CreateTime, false))?.ToList() ?? new List<Infrastructure.Entity.Coins.Coin>();
            return _mapper.Map<List<CoinShortDisplayModel>>(entities);
        }
    }
}
