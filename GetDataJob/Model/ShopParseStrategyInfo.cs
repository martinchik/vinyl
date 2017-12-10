using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetDataJob.Model
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
    }
}
