using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vinyl.Common;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Repository;
using Vinyl.Metadata;
using Vinyl.ParsingJob.Parsers;
using Vinyl.ParsingJob.Parsers.ExcelParsers;
using Vinyl.ParsingJob.Parsers.GoogleDriveParsers;
using Vinyl.ParsingJob.Parsers.HtmlParsers;

namespace Vinyl.ParsingJob.Data
{
    public class ShopStrategiesService : IShopStrategiesService
    {
        private readonly IList<Type> strategies;

        private readonly IHtmlDataGetter _htmlDataGetter;
        private readonly ILogger _logger;
        private readonly IMetadataRepositoriesFactory _metadataFactory;

        public ShopStrategiesService(ILogger<ShopStrategiesService> logger, IHtmlDataGetter htmlDataGetter, IMetadataRepositoriesFactory metadataFactory)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var strategyType = typeof(IParserStrategy);
            strategies = strategyType.Assembly.GetTypes().Where(_ => _.GetInterfaces().Contains(strategyType)).ToList();
        }

        private List<(ShopInfo shop, ShopParseStrategyInfo strategy)> GetStrategies(CancellationToken token)
        {
            ValidateStrategies(token);

            using (var strategiesRepository = _metadataFactory.CreateShopParseStrategyInfoRepository())
            {
                return strategiesRepository
                    .GetAllWithShops()
                    .AsEnumerable()
                    .Select(_ => (shop: _.Shop.ToMetaData(), strategy: _.ToMetaData()))
                    .ToList();
            }
        }

        private void ValidateStrategies(CancellationToken token)
        {
            using (var shopsRepository = _metadataFactory.CreateShopInfoRepository())
            using (var strategiesRepository = _metadataFactory.CreateShopParseStrategyInfoRepository())
            {
                var shops = shopsRepository.GetAll().ToList();
                var strategies = strategiesRepository.GetAll().ToList();

                if (
                    ValidateAndAddShop(FirstData.GetLongPlayShop(), shops, shopsRepository) ||
                    ValidateAndAddShop(FirstData.GetMuzRayShop(), shops, shopsRepository) ||
                    ValidateAndAddShop(FirstData.GetVinylShopShop(), shops, shopsRepository) ||
                    ValidateAndAddShop(FirstData.GetTanyaOnlinerShop(), shops, shopsRepository)
                    )
                {
                    shopsRepository.Commit();
                }
            }
        }

        private bool ValidateAndAddShop(DbLayer.Models.ShopInfo shopInfo, List<DbLayer.Models.ShopInfo> list, ShopInfoRepository repository)
        {
            if (list.Any(_ => _.Id == shopInfo.Id))
                return false;

            repository.Add(shopInfo);
            return true;
        }

        public void UpdateStartegyStatus(ShopParseStrategyInfo strategyInfo, int count)
        {
            using (var strategiesRepository = _metadataFactory.CreateShopParseStrategyInfoRepository())
            {
                strategiesRepository.UpdateLastProcessedCount(strategyInfo.Id, count);
                strategiesRepository.Commit();
            }
        }

        public IEnumerable<(ShopParseStrategyInfo info, IParserStrategy strategy)> GetStrategiesForRun()
        {
            foreach (var strategyInfo in GetStrategies(CancellationToken.None))
            {
                if (!CanRunStrategy(strategyInfo.strategy))
                    continue;

                if (!ValidateStrategyParametersBy(strategyInfo.shop, strategyInfo.strategy))
                    continue;

                var strategy = GetStrategyBy(strategyInfo.shop, strategyInfo.strategy);
                if (strategy == null)
                    continue;

                yield return (strategyInfo.strategy, strategy);
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
                case "VinylShopExcelParserStrategy":
                    return new VinylShopExcelParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
.Initialize(strategyInfo.Url, strategyInfo.Parameters["class-name"], strategyInfo.Parameters["ref-link-text"]);
                case "VinylShopMMExcelParserStrategy":
                    return new VinylShopMMExcelParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
.Initialize(strategyInfo.Url, strategyInfo.Parameters["class-name"], strategyInfo.Parameters["ref-link-text"]);
                case "LongPlayHtmlParserStrategy":
                    return new LongPlayHtmlParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
.Initialize(strategyInfo.Url);
                case "VinylShopHtmlParserStrategy":
                    return new VinylShopHtmlParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
.Initialize(strategyInfo.Url);
                case "VinylMuzRayGoogleExcelParserStrategy":
                    return new VinylMuzRayGoogleExcelParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
.Initialize(strategyInfo.Url);
                case "TanyaOnlinerPostParserStrategy":
                    return new Parsers.OnlinerParsers.TanyaOnlinerPostParserStrategy(_logger, _htmlDataGetter, strategyInfo.DataLimit)
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
                case "VinylMuzRayGoogleExcelParserStrategy":
                    if (string.IsNullOrWhiteSpace(strategyInfo.Url)) return ValidationFailed(strategyInfo, "Url isn't exist");
                    else return true;
                case "TanyaOnlinerPostParserStrategy":
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

    }
}
