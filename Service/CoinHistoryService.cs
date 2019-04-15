using AutoMapper;
using Infrastructure.Entity.Coins;
using Infrastructure.Interface.Repository;
using Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using MRCryptocompareClient.Client;
using MRCryptocompareClient.Infrastructure.Model.Historical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class CoinHistoryService
    {
        protected ICoinRepository _coinRepository { get; set; }
        protected ICoinHistoryRepository _coinHistoryRepository { get; set; }
        protected IMapper _mapper { get; set; }
        protected CryptocompareClient _cryptocompareClient { get; set; }
        protected readonly CryptoSyncSettings _cryptoSyncSettings;
        protected ILogger _logger { get; set; }

        const string TSYM = "USD";

        protected static bool isInProgress = false;

        public CoinHistoryService(ILoggerFactory loggerFactory, ICoinRepository coinRepository, ICoinHistoryRepository coinHistoryRepository, IMapper mapper,
            CryptocompareClient cryptocompareClient, CryptoSyncSettings cryptoSyncSettings)
        {
            _coinRepository = coinRepository;
            _coinHistoryRepository = coinHistoryRepository;
            _mapper = mapper;
            _cryptocompareClient = cryptocompareClient;
            _cryptoSyncSettings = cryptoSyncSettings;
            _logger = loggerFactory.CreateLogger(nameof(CoinHistoryService));
        }

        public void Action() => ActionAsync().Wait();

        public async Task ActionAsync()
        {
            var allDbCoins = await _coinRepository.Get(x => true);
            if (allDbCoins == null || !allDbCoins.Any())
            {
                _logger.LogError("No coinst in db found");
                return;
            }

            if (isInProgress)
                return;

            isInProgress = true;

            List<CoinHistory> histryToAdd = new List<CoinHistory>();

            try
            {
                foreach (var dbCoin in allDbCoins)
                {
                    foreach (var accent in _cryptoSyncSettings.Accent)
                    {
                        if (dbCoin.Symbol == accent)
                            continue;

                        string accentId = allDbCoins.FirstOrDefault(x => x.Symbol == accent)?.Id;

                        var request = new HistoricalAny
                        {
                            Fsym = dbCoin.Symbol,
                            Tsym = accent,
                            ToTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        };

                        CoinHistory lastMark = null;
                        if (!string.IsNullOrWhiteSpace(accentId))
                        {
                            lastMark = await _coinHistoryRepository.GetFirstSorted(x => x.CoinId == dbCoin.Id && x.AccentCoinId == accentId, x => x.Time, true);
                        }
                        else
                        {
                            lastMark = await _coinHistoryRepository.GetFirstSorted(x => x.CoinId == dbCoin.Id && x.AccentLabel == accent, x => x.Time, true);
                        }

                        if (lastMark == null)
                        {
                            request.Limit = 2000;
                        }
                        else
                        {
                            var hoursDelta = int.Parse(Math.Round(DateTime.UtcNow.Subtract(lastMark.TimeObject.ToUniversalTime()).TotalHours).ToString());

                            if (hoursDelta < 1)
                                continue;

                            request.Limit = hoursDelta > 2000 ? 2000 : hoursDelta;
                        }

                        var response = await _cryptocompareClient.Historical.Hourly(request);

                        if (!response.IsSuccess)
                            throw new Exception(response.Message);

                        var newHistory = _mapper.Map<List<CoinHistory>>(response.Data);
                        newHistory.ForEach(x => x.CoinId = dbCoin.Id);

                        if (lastMark != null)
                        {
                            newHistory.RemoveAll(x => x.TimeObject.ToUniversalTime() <= lastMark.TimeObject);
                        }

                        newHistory.ForEach((x) =>
                        {
                            if (!string.IsNullOrWhiteSpace(accentId))
                            {
                                x.Type = Infrastructure.Entity.Enum.CoinHistoryType.COIN_TO_COIN;
                                x.AccentCoinId = accentId;
                            }
                            else
                            {
                                x.Type = Infrastructure.Entity.Enum.CoinHistoryType.COIN_TO_CURRENCY;
                            }

                            x.AccentLabel = accent;
                        });

                        histryToAdd.AddRange(newHistory);
                    }
                }

                if (histryToAdd.Any())
                {
                    await _coinHistoryRepository.Insert(histryToAdd);
                    _logger.LogInformation($"Added {histryToAdd.Count} information blocks for {allDbCoins.Count()} coins");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can not add coins price history");
            }
            finally
            {
                isInProgress = false;
            }
        }

        protected int ToUnix(DateTime time)
            => (int)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }
}
