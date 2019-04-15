using MRMongoTools.Component;
using MRMongoTools.Infrastructure.Attr;
using MRMongoTools.Infrastructure.Interface;

namespace Infrastructure.Entity.Coins
{
    [CollectionAttr("Coin")]
    public class Coin : MREntity, IEntity
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public string RelativeUrl { get; set; }
        public string RelativeImageUrl { get; set; }

        public double NetHashesPerSecond { get; set; }
        public long BlockNumber { get; set; }
        public double TotalCoinsMined { get; set; }
        public bool IsTrading { get; set; }
        public bool Sponsored { get; set; }
        public int SortOrder { get; set; }
        public string TotalCoinsFreeFloat { get; set; }
        public string PreMinedValue { get; set; }
        public string SmartContactAddress { get; set; }
        public long BlockReward { get; set; }
        public string BuildOn { get; set; }
        public string FullyPremined { get; set; }
        public string ProofType { get; set; }
        public string Algorithm { get; set; }
        public string FullName { get; set; }
        public string CoinName { get; set; }
        public string TotalCoinSupply { get; set; }
        public long BlockTime { get; set; }
    }
}
