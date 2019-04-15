using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure.Entity.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoinHistoryType
    {
        COIN_TO_COIN,
        COIN_TO_CURRENCY
    }
}
