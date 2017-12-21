using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Processor
{
    public interface IDirtyRecordImportProcessor
    {
        bool ProcessRecord(DirtyRecord msg);
    }
}
