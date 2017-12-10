using GetDataJob.Model;
using GetDataJob.Parsers;
using GetDataJob.Parsers.HtmlParsers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetDataJob.Processor
{
    public class ParsingProcessor
    {
        private readonly IList<Type> strategies;

        protected readonly IHtmlDataGetter _htmlDataGetter;
        protected readonly IDirtyRecordProcessor _recordProcessor;
        protected readonly ILogger _logger;

        public ParsingProcessor(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor)
        {
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
            _recordProcessor = recordProcessor ?? throw new ArgumentNullException(nameof(recordProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var strategyType = typeof(IParserStrategy);
            strategies = strategyType.Assembly.GetTypes().Where(_ => _.GetInterfaces().Contains(strategyType)).ToList();
        }

        public void Process(IEnumerable<ShopInfo> shops)
        {
            foreach (var shopInfo in shops)
            {
                if (shopInfo.Strategies?.Any() != true)
                    continue;

                foreach (var strategyInfo in shopInfo.Strategies)
                {
                    if (string.IsNullOrEmpty(strategyInfo.ClassName))
                        continue;

                    if (!ValidateStrategyParametersBy(shopInfo, strategyInfo))
                        continue;

                    var strategy = GetStrategyBy(shopInfo, strategyInfo);
                    if (strategy == null)
                        continue;

                    Run(strategy);
                }
            }
        }

        private IParserStrategy GetStrategyBy(ShopInfo shopInfo, ShopParseStrategyInfo strategyInfo)
        {
            switch (strategyInfo.ClassName)
            {
                case "VinylShopExcelParserStrategy": return
                    new VinylShopExcelParserStrategy(_logger, _htmlDataGetter, _recordProcessor)
                        .Initialize(strategyInfo.Url, strategyInfo.Parameters["class-name"], strategyInfo.Parameters["ref-link-text"]);
                default:
                    return null;
            }
        }

        private bool ValidateStrategyParametersBy(ShopInfo shopInfo, ShopParseStrategyInfo strategyInfo)
        {
            if (shopInfo == null)
                throw new ArgumentNullException(nameof(shopInfo));
            if (strategyInfo == null)
                throw new ArgumentNullException(nameof(strategyInfo));

            switch (strategyInfo.ClassName)
            {
                case "VinylShopExcelParserStrategy":
                    if (strategyInfo.Parameters == null) return ValidationFailed(strategyInfo, "Parameters should be filled");
                    else if (!strategyInfo.Parameters.ContainsKey("class-name")) return ValidationFailed(strategyInfo, "Key 'class-name' isn't exist in parameters");
                    else if (!strategyInfo.Parameters.ContainsKey("ref-link-text")) return ValidationFailed(strategyInfo, "Key 'ref-link-text' isn't exist in parameters");
                    return true;
                default:
                    return true;
            }
        }

        private bool ValidationFailed(ShopParseStrategyInfo strategyInfo, string text)
        {
            _logger.LogWarning($"Strategy parser {strategyInfo.Id} in shop {strategyInfo.ShopId} has wrong setup and can't be run. Error={text}");
            return false;
        }

        private void Run(IParserStrategy strategy)
        {
            try
            {
                strategy.Run();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error in running parser strategy");
            }
        }
    }
}
