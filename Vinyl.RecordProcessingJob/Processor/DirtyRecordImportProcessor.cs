using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;
using Vinyl.Metadata;
using Vinyl.RecordProcessingJob.Data;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class DirtyRecordImportProcessor : IDirtyRecordImportProcessor
    {
        private readonly ILogger _logger;
        private readonly IRecordService _recordService;
        private readonly IAdditionalInfoSearchEngine _additionalInfoSearchEngine;
        private readonly IMetadataRepositoriesFactory _metadataFactory;
        private readonly IMemoryCache _cache;

        public DirtyRecordImportProcessor(ILogger<DirtyRecordImportProcessor> logger, 
            IRecordService recordService, IAdditionalInfoSearchEngine additionalInfoSearchEngine,
            IMetadataRepositoriesFactory metadataFactory,
            IMemoryCache memoryCache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _additionalInfoSearchEngine = additionalInfoSearchEngine ?? throw new ArgumentNullException(nameof(additionalInfoSearchEngine));
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
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
                    return false;

                var strategy = GetStrategyInfo(dirtyRecord.ShopParseStrategyId);

                var link = _recordService.UpdateOrCreateShopLinkInfoBy(dirtyRecord, record, strategy, out bool hasImportantChanges);
                if (link == null)
                    return false;

                if (isNewRecord)
                {
                    _additionalInfoSearchEngine.AddToSearchQueue(record.Id);
                }

                _recordService.UpdateOrCreateSearchItem(record, isNewRecord, hasImportantChanges);
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
            if (!_cache.TryGetValue(strategyId, out strategy))
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
                _cache.Set(strategyId, strategy, cacheEntryOptions);
            }

            return strategy;
        }
    }
}
