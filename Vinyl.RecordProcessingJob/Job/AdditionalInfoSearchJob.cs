using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common.Job;
using System.Threading;
using Vinyl.Metadata;
using Vinyl.Kafka;
using Vinyl.RecordProcessingJob.Processor;
using Vinyl.DbLayer;

namespace Vinyl.RecordProcessingJob.Job
{
    public class AdditionalInfoSearchJob : Vinyl.Common.Job.Job
    {
        private readonly IMessageConsumer<FindInfosRecord> _messageBus;
        private readonly IAdditionalInfoSearchEngine _searchEngine;

        public const string Name = "additional-info-job";

        private long _recivedCount;
        private long _successCount;
        private long _failedCount;
        private DateTime _lastProcessingTime;

        public AdditionalInfoSearchJob(ILogger<AdditionalInfoSearchJob> logger, IMessageConsumer<FindInfosRecord> messageBus,
            IAdditionalInfoSearchEngine searchEngine) :
            base(logger, Name, null)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _searchEngine = searchEngine ?? throw new ArgumentNullException(nameof(searchEngine));
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            _lastProcessingTime = DateTime.UtcNow;

            return Task.Run(() => 
                _messageBus.SubscribeOnTopic(ProcessKafkaMessage, () =>
                {
                    if ((_lastProcessingTime - DateTime.UtcNow).TotalHours == 6 && _recivedCount != 0)
                    {
                        _recivedCount = 0;
                        _successCount = 0;
                        _failedCount = 0;
                    }
                }, token),
                token);
        }

        private void ProcessKafkaMessage(FindInfosRecord msg, string message)
        {
            _lastProcessingTime = DateTime.UtcNow;

            Interlocked.Increment(ref _recivedCount);

            Logger.LogTrace("Kafka recieved msg:" + message);

            if (_searchEngine.Search(msg))
            {
                Interlocked.Increment(ref _successCount);
            }
            else
            {
                Interlocked.Increment(ref _failedCount);

                Logger.LogTrace("Failed to search additional infos:" + msg.ToString());
            }

            Result = $"Recieved {_recivedCount} records (successed:{_successCount} and failed:{_failedCount})";
        }
    }
}
