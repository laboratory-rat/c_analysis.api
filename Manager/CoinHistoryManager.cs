using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Entity.Coins;
using Infrastructure.Entity.User;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.CoinHistory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRIdentityClient.Exception.Common;
using MRMongoTools.Extensions.Identity.Interface;

namespace Manager
{
    public class CoinHistoryManager : BaseManager, ICoinHistoryManager
    {
        protected readonly ICoinHistoryRepository _coinHistoryRepository;
        protected readonly ICoinRepository _coinRepository;

        public CoinHistoryManager(ILoggerFactory _loggerFactory, IHttpContextAccessor httpContextAccessor, 
            IMapper mapper, IdentityUserManager identityUserManager, 
            IMRUserStore<UserEntity> mrUserStore, ICoinHistoryRepository coinHistoryRepository, 
            ICoinRepository coinRepository) : base(_loggerFactory, httpContextAccessor, mapper, identityUserManager, mrUserStore)
        {
            _coinHistoryRepository = coinHistoryRepository;
            _coinRepository = coinRepository;
        }

        public async Task<CoinHistoryDisplayModel> Search(CoinHistorySearchModel model)
        {
            var primary = await _coinRepository.Get(model.Primary);
            if (primary == null)
                throw new EntityNotFoundException<Coin>(model.Primary);

            string accent = model.Accent;

            if(model.Until.HasValue && model.Until < model.From)
            {
                var tmp = model.Until.Value;
                model.Until = model.From;
                model.From = tmp;
            }

            Expression<Func<CoinHistory, bool>> searchExpression = 
                x => x.CoinId == primary.Id && x.State == MRMongoTools.Infrastructure.Enum.EntityState.Active && x.Type == model.Type;
            var compiledSearch = searchExpression.Compile();

            if (model.Type == Infrastructure.Entity.Enum.CoinHistoryType.COIN_TO_COIN)
                searchExpression = x => compiledSearch(x) && x.AccentCoinId == accent;
            else
                searchExpression = x => compiledSearch(x) && x.AccentLabel == accent;

            var queryResult = await _coinHistoryRepository.GetSorted(searchExpression, x => x.Time, true);

            var result = new CoinHistoryDisplayModel
            {
                PrimaryLabel = primary.Name,
                Units = new System.Collections.Generic.List<CoinHistoryUnitDisplayModel>()
            };

            if(model.Type == Infrastructure.Entity.Enum.CoinHistoryType.COIN_TO_COIN)
            {
                var accentModel = await _coinRepository.Get(model.Accent);
                result.AccentLabel = accentModel?.Name;
            }
            else
            {
                result.AccentLabel = accent;
            }

            if (queryResult == null || !queryResult.Any())
                return result;

            result.Units = _mapper.Map<List<CoinHistoryUnitDisplayModel>>(queryResult);
            return result;
        }
    }
}
