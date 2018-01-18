using Microsoft.Extensions.Logging;
using System;
using Vinyl.Kafka;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Processor
{
    public class DirtyRecordExportProcessor : IDirtyRecordExportProcessor
    {
        private readonly ILogger _logger;
        private readonly IMessageProducer<DirtyRecord> _messageBus;

        public DirtyRecordExportProcessor(ILogger<DirtyRecordExportProcessor> logger, IMessageProducer<DirtyRecord> messageBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }
        
        public bool AddRecord(ShopParseStrategyInfo strategy, DirtyRecord record)
        {
            if (record == null || strategy == null ||
                string.IsNullOrEmpty(record.Album) ||
                string.IsNullOrEmpty(record.Artist))
                return false;

            record.ShopId = strategy.ShopId;
            record.ShopParseStrategyId = strategy.Id;

            _messageBus.SendMessage(record).ContinueWith(_ => 
            {
                if (_.IsCompletedSuccessfully)
                    _logger.LogTrace("Kafka send DirtyRecord msg:" + _.Result);
                else
                    _logger.LogError(_.Exception, "Error has occurred when message DirtyRecord was sending to kafka");
            });
            return true;
        }        
    }
}
