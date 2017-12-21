using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer.Models;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Data
{
    public interface IRecordService
    {
        RecordInfo FindOrCreateSimilarBy(DirtyRecord dirtyRecord);
        RecordInShopLink CreateOrUpdateShopLinkInfoBy(DirtyRecord dirtyRecord, RecordInfo record);
    }
}