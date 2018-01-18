using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;
using Vinyl.DbLayer.Repository;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class AdditionalInfoSearchEngine : IAdditionalInfoSearchEngine
    {
        private readonly ILogger _logger;
        private readonly IMetadataRepositoriesFactory _metadataFactory;
        private RecordInfoRepository _recordRepository;

        public AdditionalInfoSearchEngine(ILogger<DirtyRecordImportProcessor> logger, IMetadataRepositoriesFactory metadataFactory)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }        

        public bool Search(FindInfosRecord record)
        {
            if (record == null || record.RecordId == Guid.Empty)
                return false;

            if (_recordRepository == null)
            {
                _recordRepository = _metadataFactory.CreateRecordInfoRepository();
            }
            return SearchProcess(record.ShopId, record.ShopParseStrategyId, _recordRepository.Get(record.RecordId));
        }

        private bool SearchProcess(Guid shopId, Guid stratefyId, RecordInfo record)
        {
            if (record == null)
                return false;

            return true;
        }
    }
}
