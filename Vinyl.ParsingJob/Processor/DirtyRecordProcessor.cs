using Vinyl.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Kafka;

namespace Vinyl.ParsingJob.Processor
{
    public class DirtyRecordProcessor : IDirtyRecordProcessor
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;

        public DirtyRecordProcessor(ILogger<DirtyRecordProcessor> logger, IMessageBus messageBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }
        
        public bool AddRecord(ShopParseStrategyInfo strategy, DirtyRecord record)
        {
            if (record == null ||
                string.IsNullOrEmpty(record.Title) ||
                string.IsNullOrEmpty(record.Artist))
                return false;

            _messageBus.SendMessage(record);
            return true;
        }        
    }
}
