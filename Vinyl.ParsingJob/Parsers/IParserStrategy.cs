using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers
{
    public interface IParserStrategy
    {
        IEnumerable<DirtyRecord> Parse(CancellationToken token = default(CancellationToken));
    }
}