using GetDataJob.Model;
using System.Collections.Generic;

namespace GetDataJob.Processor
{
    public interface IDirtyRecordProcessor
    {
        void AddRecord(string strategyName, DirtyRecord record);
        IEnumerable<string> GetCsvLines();
    }
}