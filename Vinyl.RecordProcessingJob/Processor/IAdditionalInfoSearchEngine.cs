using System;

namespace Vinyl.RecordProcessingJob.Processor
{
    public interface IAdditionalInfoSearchEngine
    {
        void AddToSearchQueue(Guid recordId);
    }
}