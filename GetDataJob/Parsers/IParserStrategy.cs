using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Parsers
{
    public interface IParserStrategy
    {
        Task Run(CancellationToken token = default(CancellationToken));
    }
}