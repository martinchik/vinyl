using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class ShopParseStrategyInfo
    {
        public ShopParseStrategyInfo()
        {
            RecordInShopLink = new HashSet<RecordInShopLink>();
        }

        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public string ClassName { get; set; }
        public string StartUrl { get; set; }
        public int UpdatePeriodInHours { get; set; }
        public int? LastProcessedCount { get; set; }
        public int? DataLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string Parameters { get; set; }
        public int Status { get; set; }

        public ShopInfo Shop { get; set; }
        public ICollection<RecordInShopLink> RecordInShopLink { get; set; }
    }
}
