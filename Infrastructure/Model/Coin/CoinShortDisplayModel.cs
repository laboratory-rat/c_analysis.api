using Infrastructure.Model.Common;

namespace Infrastructure.Model.Coin
{
    public class CoinShortDisplayModel : IdNameModel
    {
        public string Symbol { get; set; }
        public string FullName { get; set; }

        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}
