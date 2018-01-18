using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class ShopInfo
    {
        public ShopInfo()
        {
            RecordInShopLink = new HashSet<RecordInShopLink>();
            ShopParseStrategyInfo = new HashSet<ShopParseStrategyInfo>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Emails { get; set; }
        public string Phones { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }

        public ICollection<RecordInShopLink> RecordInShopLink { get; set; }
        public ICollection<ShopParseStrategyInfo> ShopParseStrategyInfo { get; set; }
    }
}
