using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.ParsingJob.Parsers
{
    public interface IParserStrategy
    {
        Task<int> Run(CancellationToken token = default(CancellationToken));
    }
}