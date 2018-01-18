using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;
using Vinyl.Kafka;
using Vinyl.Metadata;
using Vinyl.RecordProcessingJob.Data;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class DirtyRecordImportProcessor : IDirtyRecordImportProcessor
    {
        private readonly ILogger _logger;
        private readonly IRecordService _recordService;
        private readonly IMessageProducer<FindInfosRecord> _messageBus;
        private readonly IMetadataRepositoriesFactory _metadataFactory;
        private readonly IMemoryCache _cacheShops;
        private readonly IMemoryCache _cacheStrategies;

        public DirtyRecordImportProcessor(ILogger<DirtyRecordImportProcessor> logger, 
            IRecordService recordService, IMessageProducer<FindInfosRecord> messageBus,
            IMetadataRepositoriesFactory metadataFactory,
            IMemoryCache memoryCacheStrategies, IMemoryCache memoryCacheShops)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(_messageBus));
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _cacheStrategies = memoryCacheStrategies ?? throw new ArgumentNullException(nameof(memoryCacheStrategies));
            _cacheShops = memoryCacheShops ?? throw new ArgumentNullException(nameof(memoryCacheShops));
        }

        public bool ProcessRecord(DirtyRecord dirtyRecord)
        {
            if (dirtyRecord == null || dirtyRecord.ShopId == Guid.Empty || dirtyRecord.ShopParseStrategyId == Guid.Empty ||
               string.IsNullOrEmpty(dirtyRecord.Album) ||
               string.IsNullOrEmpty(dirtyRecord.Artist))
               return false;

            try
            {
                RecordInfo record = _recordService.FindOrCreateSimilarBy(dirtyRecord, out bool isNewRecord);
                if (record == null)
                {
                    _logger.LogWarning($"Record was not saved. {dirtyRecord.ToString()}");
                    return false;
                }

                var shop = GetShopInfo(dirtyRecord.ShopId);
                var strategy = GetStrategyInfo(dirtyRecord.ShopParseStrategyId);
                if (shop == null || strategy == null)
                {
                    _logger.LogWarning($"Shop or strategy was not found. Record was not saved. {dirtyRecord.ToString()}");
                    return false;
                }

                var link = _recordService.UpdateOrCreateShopLinkInfoBy(dirtyRecord, record, strategy, out bool hasImportantChanges);
                if (link == null)
                    return false;

                if (isNewRecord)
                {
                    _messageBus.SendMessage(new FindInfosRecord()
                    {
                        ShopId = shop.Id,
                        ShopParseStrategyId = strategy.Id,
                        RecordId = record.Id
                    }).ContinueWith(_ =>
                    {
                        if (_.IsCompletedSuccessfully)
                            _logger.LogTrace("Kafka send FindInfosRecord msg:" + _.Result);
                        else
                            _logger.LogError(_.Exception, "Error has occurred when message FindInfosRecord was sending to kafka");
                    });
                }

                _recordService.UpdateOrCreateSearchItem(record, shop.CountryCode, isNewRecord, hasImportantChanges);
                return true;
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Faild when processing dirty record");
            }

            return false;
        }

        private Metadata.ShopParseStrategyInfo GetStrategyInfo(Guid strategyId)
        {
            Metadata.ShopParseStrategyInfo strategy;

            // Look for cache key.
            if (!_cacheStrategies.TryGetValue(strategyId, out strategy))
            {
                using (var repo = _metadataFactory.CreateShopParseStrategyInfoRepository())
                {
                    // Key not in cache, so get data.
                    strategy = repo.Get(strategyId).ToMetaData();
                }

                if (strategy == null)
                    throw new Exception($"Strategy info '{strategyId}' did not find in repository");

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromHours(3));

                // Save data in cache.
                _cacheStrategies.Set(strategyId, strategy, cacheEntryOptions);
            }

            return strategy;
        }

        private Metadata.ShopInfo GetShopInfo(Guid shopId)
        {
            Metadata.ShopInfo shop;

            // Look for cache key.
            if (!_cacheShops.TryGetValue(shopId, out shop))
            {
                using (var repo = _metadataFactory.CreateShopInfoRepository())
                {
                    // Key not in cache, so get data.
                    shop = repo.Get(shopId).ToMetaData();
                }

                if (shop == null)
                    throw new Exception($"Shop info '{shopId}' did not find in repository");

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromHours(3));

                // Save data in cache.
                _cacheStrategies.Set(shopId, shop, cacheEntryOptions);
            }

            return shop;
        }
    }
}
