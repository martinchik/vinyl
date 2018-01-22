using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class RecordInShopLink
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public Guid StrategyId { get; set; }
        public Guid RecordId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceBy { get; set; }
        public string Currency { get; set; }
        public string ShopInfo { get; set; }
        public string ShopRecordTitle { get; set; }
        public string ShopImageUrl { get; set; }
        public string ShopUrl { get; set; }
        public string State { get; set; }
        public string Barcode { get; set; }
        public string Country { get; set; }
        public string CountInPack { get; set; }
        public string YearRecorded { get; set; }
        public string Label { get; set; }
        public string Style { get; set; }
        public string ViewType { get; set; }
        public int Status { get; set; }

        public RecordInfo Record { get; set; }
        public ShopInfo Shop { get; set; }
        public ShopParseStrategyInfo Strategy { get; set; }
    }
}
