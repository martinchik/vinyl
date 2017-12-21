using Vinyl.Metadata;
using System.Collections.Generic;

namespace Vinyl.ParsingJob.Processor
{
    public interface IDirtyRecordExportProcessor
    {
        bool AddRecord(ShopParseStrategyInfo strategy, DirtyRecord record);
    }
}