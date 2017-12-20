using System.Collections.Generic;
using Vinyl.ParsingJob.Parsers;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Data
{
    public interface IShopStrategiesService
    {
        IEnumerable<(ShopParseStrategyInfo info, IParserStrategy strategy)> GetStrategiesForRun();
        void UpdateStartegyStatus(ShopParseStrategyInfo strategyInfo, int count);
    }
}