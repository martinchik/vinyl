using System.Collections.Generic;
using Vinyl.ParsingJob.Parsers;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Data
{
    public interface IShopStrategiesService
    {
        IEnumerable<IParserStrategy> GetStrategiesForRun(IEnumerable<ShopInfo> shops);
    }
}