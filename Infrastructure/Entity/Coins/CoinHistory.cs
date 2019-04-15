using Infrastructure.Entity.Enum;
using MongoDB.Bson.Serialization.Attributes;
using MRMongoTools.Component;
using MRMongoTools.Infrastructure.Attr;
using MRMongoTools.Infrastructure.Interface;
using System;

namespace Infrastructure.Entity.Coins
{
    [CollectionAttr("CoinHistory")]
    public class CoinHistory : MREntity, IEntity
    {
        public string CoinId { get; set; }
        public string AccentCoinId { get; set; }
        public string AccentLabel { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public CoinHistoryType Type { get; set; }

        public long Time { get; set; }
        public DateTime TimeObject { get; set; }

        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }

        public float VolumeFrom { get; set; }
        public float VolumeTo { get; set; }
    }
}
