using Vinyl.Metadata;
using System.Collections.Generic;

namespace Vinyl.GetDataJob.Processor
{
    public interface IDirtyRecordProcessor
    {
        void AddRecord(string strategyName, DirtyRecord record);
        IEnumerable<string> GetCsvLines();
    }
}