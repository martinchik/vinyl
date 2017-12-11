using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.GetDataJob.Parsers
{
    public interface IParserStrategy
    {
        Task Run(CancellationToken token = default(CancellationToken));
    }
}