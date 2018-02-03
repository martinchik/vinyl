using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Common
{
    public interface IHtmlDataGetter
    {
        Task<string> GetPage(string url, CancellationToken token = default(CancellationToken), bool useEncoding = false);
        Task<string> SendAsync(string url, AuthenticationHeaderValue authenticationHeader = null, CancellationToken token = default(CancellationToken));
        Task<string> DownloadFileToLocal(string fileUrl, CancellationToken token = default(CancellationToken));
    }
}