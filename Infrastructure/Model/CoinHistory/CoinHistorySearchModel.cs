using Infrastructure.Entity.Enum;
using Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model.CoinHistory
{
    public class CoinHistorySearchModel
    {
        [Required]
        public string Primary { get; set; }
        [Required]
        public string Accent { get; set; }

        [Required]
        public DateTime From { get; set; }
        public DateTime? Until { get; set; }

        [Required]
        public CoinHistoryType Type { get; set; }

        [Required]
        public CoinHistoryResolution Resulution { get; set; }
    }
}
