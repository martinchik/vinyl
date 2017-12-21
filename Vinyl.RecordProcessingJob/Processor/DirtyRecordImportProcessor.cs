using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer.Models;
using Vinyl.Metadata;
using Vinyl.RecordProcessingJob.Data;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class DirtyRecordImportProcessor : IDirtyRecordImportProcessor
    {
        private readonly ILogger _logger;
        private readonly IRecordService _recordService;

        public DirtyRecordImportProcessor(ILogger<DirtyRecordImportProcessor> logger, IRecordService recordService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recordService = recordService ?? throw new ArgumentNullException(nameof(recordService));
        }

        public bool ProcessRecord(DirtyRecord dirtyRecord)
        {
            if (dirtyRecord == null || dirtyRecord.ShopId == Guid.Empty || dirtyRecord.ShopParseStrategyId == Guid.Empty ||
               string.IsNullOrEmpty(dirtyRecord.Album) ||
               string.IsNullOrEmpty(dirtyRecord.Artist))
               return false;

            try
            {
                RecordInfo record = _recordService.FindOrCreateSimilarBy(dirtyRecord);
                if (record == null)
                    return false;

                var link = _recordService.CreateOrUpdateShopLinkInfoBy(dirtyRecord, record);

                return true;
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Faild when processing dirty record");
            }

            return false;
        }
    }
}
