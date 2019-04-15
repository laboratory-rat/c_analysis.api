using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Infrastructure.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoinHistoryResolution
    {
        DAY,
        MINUTES_30
    }
}
