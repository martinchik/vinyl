using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace Vinyl.ParsingJob.Tests
{
    [TestClass]
    public class ParserStrategiesOriginalTests
    {               
        [TestMethod]
        [Ignore("Original")]
        public void LongPlay_HtmlParser_Strategy_Original_Test()
        {
            //var htmlGetter = new HtmlDataGetter(Substitute.For<ILogger<HtmlDataGetter>>());
            //var recordProcessor = new DirtyRecordProcessor(Substitute.For<ILogger<DirtyRecordProcessor>>(), Substitute.For<IMessageBus>());

            //var shops = new[] { Vinyl.Metadata.Test.TestShops.GetLongPlayShop() };
            //IParserStrategy strategy = (new ShopStrategiesService(Substitute.For<ILogger<ShopStrategiesService>>(), htmlGetter, recordProcessor)).GetStrategiesForRun(shops).FirstOrDefault();
            //strategy.Parse(CancellationToken.None).GetAwaiter().GetResult();

            //SaveResultsTo(strategy.GetType().Name + ".csv", recordProcessor);
        }        
    }
}
