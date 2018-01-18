using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Kafka;
using Vinyl.ParsingJob.Data;
using Vinyl.ParsingJob.Parsers;
using Vinyl.ParsingJob.Parsers.HtmlParsers;
using Vinyl.ParsingJob.Processor;
using Vinyl.RecordProcessingJob.Data;

namespace Vinyl.ParsingJob.Tests
{
    [TestClass]
    public class ParseSpecialFieldsTests
    {        
        [TestMethod()]
        public void ParseRecordName_Test(string recordFrom, string recordTo)
        {
            var res = ParseSpecialFields.ParseRecordName(recordFrom);

            res.Should().Be(recordTo);
        }        
    }
}
