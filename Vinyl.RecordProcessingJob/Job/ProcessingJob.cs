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

namespace Vinyl.RecordProcessingJob.Job
{
    public class ProcessingJob : Vinyl.Common.Job.Job
    {
        private readonly IMessageConsumer<DirtyRecord> _messageBus;
        private readonly IDirtyRecordImportProcessor _importProcessor;

        public const string Name = "processing-job";

        private long _recivedCount;
        private long _successCount;
        private long _failedCount;
        private DateTime _lastProcessingTime;

        public ProcessingJob(ILogger<ProcessingJob> logger, IMessageConsumer<DirtyRecord> messageBus, IDirtyRecordImportProcessor importProcessor) :
            base(logger, Name, null)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _importProcessor = importProcessor ?? throw new ArgumentNullException(nameof(importProcessor));
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

        private void ProcessKafkaMessage(DirtyRecord msg, string message)
        {
            _lastProcessingTime = DateTime.UtcNow;

            Interlocked.Increment(ref _recivedCount);

            Logger.LogTrace("Kafka recieved msg:" + message);

            if (_importProcessor.ProcessRecord(msg))
            {
                Interlocked.Increment(ref _successCount);
            }
            else
            {
                Interlocked.Increment(ref _failedCount);

                Logger.LogTrace("Failed to process dirty record:" + msg.ToString());
            }

            Result = $"Recieved {_recivedCount} records (successed:{_successCount} and failed:{_failedCount})";
        }
    }
}
