using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vinyl.Metadata
{
    public class ShopInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public ShopParseStrategyInfo[] Strategies { get; set; }
        public string CountryCode { get; set; }        
    }
}
