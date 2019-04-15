using AutoMapper;
using Infrastructure.Entity.Coins;
using Infrastructure.Entity.User;
using Infrastructure.Model.Coin;
using Infrastructure.Model.CoinHistory;
using Infrastructure.Model.User;
using MRMongoTools.Extensions.Identity.Component;
using System;

namespace MRCryptoCurrencyAnalysis.Init
{
    public class UserProfile : Profile
    {
        const string CRYPTOCOMPARE = "http://cryptocompare.com";
        string ToAbsolute(string url) => $"{CRYPTOCOMPARE}{url}";

        public UserProfile()
        {
            CreateMap<MRCryptocompareClient.Infrastructure.Response.GeneralCoin, Coin>()
                .ForMember(x => x.ExternalId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.RelativeUrl, opt => opt.MapFrom(x => x.Url))
                .ForMember(x => x.RelativeImageUrl, opt => opt.MapFrom(x => x.ImageUrl))
                .ForMember(x => x.SortOrder, opt => opt.Ignore());

            CreateMap<MRCryptocompareClient.Infrastructure.Response.HistoricalData, CoinHistory>()
                .ForMember(x => x.TimeObject, opt => opt.MapFrom(x => DateTimeOffset.FromUnixTimeSeconds(x.Time).ToUniversalTime()));

            // user
            CreateMap<UserEntity, UserLoginResponse>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => x.Image == null ? string.Empty : x.Image.Url));

            // coin
            CreateMap<Coin, CoinShortDisplayModel>()
                .ForMember(x => x.Url, opt => opt.MapFrom(x => ToAbsolute(x.RelativeUrl)))
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(x => ToAbsolute(x.RelativeImageUrl)));

            // history
            CreateMap<CoinHistory, CoinHistoryUnitDisplayModel>();
        }
    }
}
