using Infrastructure.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Model.CoinHistory
{
    public class CoinHistoryDisplayModel
    {
        public string PrimaryLabel { get; set; }
        public string AccentLabel { get; set; }
        public List<CoinHistoryUnitDisplayModel> Units { get; set; }
    }

    public class CoinHistoryUnitDisplayModel
    {
        public CoinHistoryType Type { get; set; }
        public DateTime TimeObject { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float Hight { get; set; }
        public float Low { get; set; }
        public float VolumeFrom { get; set; }
        public float VolumeTo { get; set; }
    }
}
