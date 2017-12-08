using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Parsers
{
    public interface IHtmlDataGetter
    {
        Task<string> GetPage(string url, CancellationToken token = default(CancellationToken));
    }
}