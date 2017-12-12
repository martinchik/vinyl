﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common.Extensions;
using Vinyl.Common.Job;
using System.Threading;
using Vinyl.Metadata;
using Vinyl.Kafka;

namespace Vinyl.RecordProcessingJob.Job
{
    public class ProcessingJob : Vinyl.Common.Job.Job
    {
        private readonly IMessageBus _messageBus;

        public const string Name = "processing-job";

        private long _recivedCount;

        public ProcessingJob(ILogger<ProcessingJob> logger, IMessageBus messageBus) :
            base(logger, Name, null)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        protected override Task ExecuteAsync(CancellationToken token)
        {
            return Task.Run(() => 
                _messageBus.SubscribeOnTopic<DirtyRecord>(ProcessKafkaMessage, token),
                token);
        }

        private void ProcessKafkaMessage(DirtyRecord msg)
        {
            Interlocked.Increment(ref _recivedCount);

            Result = $"Recieved {_recivedCount} records";

            Logger.LogInformation($"Recieved msg:{msg.ToString()}");
        }
    }
}