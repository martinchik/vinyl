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
        private readonly IMessageConsumer _messageBus;
        private readonly IDirtyRecordImportProcessor _importProcessor;

        public const string Name = "processing-job";

        private long _recivedCount;
        private long _successCount;
        private long _failedCount;

        public ProcessingJob(ILogger<ProcessingJob> logger, IMessageConsumer messageBus, IDirtyRecordImportProcessor importProcessor) :
            base(logger, Name, null)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _importProcessor = importProcessor ?? throw new ArgumentNullException(nameof(importProcessor));
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            return Task.Run(() => 
                _messageBus.SubscribeOnTopic<DirtyRecord>(ProcessKafkaMessage, token),
                token);
        }

        private void ProcessKafkaMessage(DirtyRecord msg, string message)
        {
            Interlocked.Increment(ref _recivedCount);

            Result = $"Recieved {_recivedCount} records";

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
