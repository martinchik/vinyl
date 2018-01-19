using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.DbLayer.Models
{
    public partial class SearchItem
    {
        public Guid Id { get; set; }
        public Guid RecordId { get; set; }
        public string TextLine1 { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public bool Sell { get; set; }
        public string TextLine2 { get; set; }
        public string CountryCode { get; set; }
        public int ShopsCount { get; set; }
        public string States { get; set; }
    }
}
