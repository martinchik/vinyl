using System.Collections.Generic;
using Vinyl.GetDataJob.Parsers;
using Vinyl.Metadata;

namespace Vinyl.GetDataJob.Data
{
    public interface IShopStrategiesService
    {
        IEnumerable<IParserStrategy> GetStrategiesForRun(IEnumerable<ShopInfo> shops);
    }
}