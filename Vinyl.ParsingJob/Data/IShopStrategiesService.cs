using System.Collections.Generic;
using Vinyl.ParsingJob.Parsers;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Data
{
    public interface IShopStrategiesService
    {
        IEnumerable<(ShopParseStrategyInfo info, IParserStrategy strategy)> GetStrategiesForRun(IEnumerable<ShopInfo> shops);
        void UpdateStartegyStatus(ShopParseStrategyInfo strategyInfo, int count);
    }
}