using System;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Processor
{
    public interface IAdditionalInfoSearchEngine
    {
        bool Search(FindInfosRecord record);
    }
}