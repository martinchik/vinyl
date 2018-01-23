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
    public class AliveValidationJob : Vinyl.Common.Job.RepeatableJob
    {
        private readonly IMetadataRepositoriesFactory _metadataFactory;

        public const string Name = "additional_info-job";
        
        public AliveValidationJob(ILogger<AdditionalInfoSearchJob> logger, IMetadataRepositoriesFactory metadataFactory) :
            base(logger, Name, Repeat.After.Minutes(1).Than(Repeat.Each.Hours(6)), TimeSpan.FromHours(1))
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {        
            using (var rep = _metadataFactory.CreateRecordInShopLinkRepository())
            {
                var recordsCount = rep.RemoveAllWhereUpdatedAtLessThen(DateTime.UtcNow.AddDays(-GlobalConstants.RecordInShopeAliveDaysCount));

                await rep.CommitAsync(token);

                Logger.LogTrace($"AliveValidationJob removed {recordsCount} record links from shops");

                Result = $"Removed {recordsCount} records";
            }            
        }        
    }
}
