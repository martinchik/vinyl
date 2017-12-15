using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vinyl.Metadata
{
    public class ShopParseStrategyInfo
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public string ClassName { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime ProcessedAt { get; set; }
        public int? DataLimit { get; set; }
        public int UpdatePeriodInHours { get; set; }
        public int LastProcessedCount { get; set; }
    }
}
