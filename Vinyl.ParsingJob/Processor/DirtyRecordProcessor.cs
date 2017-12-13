﻿using Vinyl.Metadata;
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
        private readonly IMessageProducer _messageBus;

        public DirtyRecordProcessor(ILogger<DirtyRecordProcessor> logger, IMessageProducer messageBus)
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

            _messageBus.SendMessage(record).ContinueWith(_ => 
            {
                if (_.IsCompletedSuccessfully)
                    _logger.LogTrace("Kafka send msg:" + _.Result);
                else
                    _logger.LogError(_.Exception, "Error has occurred when message was sending to kafka");
            });
            return true;
        }        
    }
}
