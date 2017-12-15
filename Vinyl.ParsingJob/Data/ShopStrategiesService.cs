using Vinyl.Metadata;
using Vinyl.ParsingJob.Parsers;
using Vinyl.ParsingJob.Parsers.HtmlParsers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.ParsingJob.Processor;

namespace Vinyl.ParsingJob.Data
{
    public class ShopStrategiesService : IShopStrategiesService
    {
        private readonly IList<Type> strategies;

        protected readonly IHtmlDataGetter _htmlDataGetter;
        protected readonly ILogger _logger;

        public ShopStrategiesService(ILogger<ShopStrategiesService> logger, IHtmlDataGetter htmlDataGetter)
        {
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var strategyType = typeof(IParserStrategy);
            strategies = strategyType.Assembly.GetTypes().Where(_ => _.GetInterfaces().Contains(strategyType)).ToList();
        }

        public IEnumerable<(ShopParseStrategyInfo info, IParserStrategy strategy)> GetStrategiesForRun(IEnumerable<ShopInfo> shops)
        {
            foreach (var shopInfo in shops)
            {
                if (shopInfo.Strategies?.Any() != true)
                    continue;

                foreach (var strategyInfo in shopInfo.Strategies)
                {
                    if (!CanRunStrategy(strategyInfo))
                        continue;

                    if (!ValidateStrategyParametersBy(shopInfo, strategyInfo))
                        continue;

                    var strategy = GetStrategyBy(shopInfo, strategyInfo);
                    if (strategy == null)
                        continue;

                    yield return (strategyInfo, strategy);
                }
            }
        }

        private bool CanRunStrategy(ShopParseStrategyInfo strategyInfo)
        {
            if (string.IsNullOrEmpty(strategyInfo.ClassName))
                return false;

            if ((DateTime.UtcNow - strategyInfo.ProcessedAt).TotalHours < strategyInfo.UpdatePeriodInHours)
                return false;

            return true;
        }

        private IParserStrategy GetStrategyBy(ShopInfo shopInfo, ShopParseStrategyInfo strategyInfo)
        {
            switch (strategyInfo.ClassName)
            {
                case "VinylShopExcelParserStrategy": return new VinylShopExcelParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
                        .Initialize(strategyInfo.Url, strategyInfo.Parameters["class-name"], strategyInfo.Parameters["ref-link-text"]);
                case "VinylShopMMExcelParserStrategy": return new VinylShopMMExcelParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
                        .Initialize(strategyInfo.Url, strategyInfo.Parameters["class-name"], strategyInfo.Parameters["ref-link-text"]);
                case "LongPlayHtmlParserStrategy": return new LongPlayHtmlParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
                        .Initialize(strategyInfo.Url);
                case "VinylShopHtmlParserStrategy": return new VinylShopHtmlParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
                        .Initialize(strategyInfo.Url);
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
                    if (string.IsNullOrWhiteSpace(strategyInfo.Url)) return ValidationFailed(strategyInfo, "Url isn't exist");
                    else if (strategyInfo.Parameters == null) return ValidationFailed(strategyInfo, "Parameters should be filled");
                    else if (!strategyInfo.Parameters.ContainsKey("class-name")) return ValidationFailed(strategyInfo, "Key 'class-name' isn't exist in parameters");
                    else if (!strategyInfo.Parameters.ContainsKey("ref-link-text")) return ValidationFailed(strategyInfo, "Key 'ref-link-text' isn't exist in parameters");
                    else return true;
                case "VinylShopMMExcelParserStrategy":
                    if (string.IsNullOrWhiteSpace(strategyInfo.Url)) return ValidationFailed(strategyInfo, "Url isn't exist");
                    else if (strategyInfo.Parameters == null) return ValidationFailed(strategyInfo, "Parameters should be filled");
                    else if (!strategyInfo.Parameters.ContainsKey("class-name")) return ValidationFailed(strategyInfo, "Key 'class-name' isn't exist in parameters");
                    else if (!strategyInfo.Parameters.ContainsKey("ref-link-text")) return ValidationFailed(strategyInfo, "Key 'ref-link-text' isn't exist in parameters");
                    else return true;
                case "LongPlayHtmlParserStrategy":
                    if (string.IsNullOrWhiteSpace(strategyInfo.Url)) return ValidationFailed(strategyInfo, "Url isn't exist");
                    else return true;
                case "VinylShopHtmlParserStrategy":
                    if (string.IsNullOrWhiteSpace(strategyInfo.Url)) return ValidationFailed(strategyInfo, "Url isn't exist");
                    else return true;
                default:
                    return true;
            }
        }

        private bool ValidationFailed(ShopParseStrategyInfo strategyInfo, string text)
        {
            _logger.LogWarning($"Strategy parser {strategyInfo.Id} in shop {strategyInfo.ShopId} has wrong setup and can't be run. Error={text}");
            return false;
        }

        public void UpdateStartegyStatus(ShopParseStrategyInfo strategyInfo, int count)
        {
            strategyInfo.ProcessedAt = DateTime.UtcNow;
            strategyInfo.LastProcessedCount = count;
        }
    }
}
