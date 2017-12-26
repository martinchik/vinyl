using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer.Models;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Data
{
    public interface IRecordService
    {
        RecordInfo FindOrCreateSimilarBy(DirtyRecord dirtyRecord, out bool isNew);
        RecordInShopLink UpdateOrCreateShopLinkInfoBy(DirtyRecord dirtyRecord, RecordInfo record, Metadata.ShopParseStrategyInfo strategy, out bool hasImportantChanges);
        bool UpdateOrCreateSearchItem(RecordInfo record, bool isNewRecord, bool hasImportantChanges);
    }
}