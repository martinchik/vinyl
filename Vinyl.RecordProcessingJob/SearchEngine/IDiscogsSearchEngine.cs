using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.RecordProcessingJob.SearchEngine
{
    public interface IDiscogsSearchEngine
    {
        Task<(string title, string img, string url, long id, string resources)?> FindBy(string barcode, CancellationToken token);
        Task<(string title, string img, string url, long id, string resources)?> FindBy(string artist, string album, string year, CancellationToken token);
        Task<DiscogsSearchEngine.Resources> GetResourcesByUrl(string url, CancellationToken token);
    }
}