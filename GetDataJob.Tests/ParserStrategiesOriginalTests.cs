using GetDataJob.Parsers;
using GetDataJob.Parsers.HtmlParsers;
using GetDataJob.Processor;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Tests
{
    [TestClass]
    public class ParserStrategiesOriginalTests
    {
        [TestMethod]
        [Ignore("Original")]
        public async Task All_Strategies_Original_Test()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            try
            {
                await new LongPlayHtmlParserStrategy(logger, htmlGetter, recordProcessor).Run(CancellationToken.None);
                await new VinylShopHtmlParserStrategy(logger, htmlGetter, recordProcessor).Run(CancellationToken.None);
            }
            finally
            {
                string fileName = @"C:\test\allData.csv";
                string[] header = new[] { "Parser;Album;Artist;Title;Year;State;Price;Info;Url" };
                if (File.Exists(fileName))
                    File.Delete(fileName);

                File.WriteAllLines(@"C:\test\allData.csv", header.Concat(recordProcessor.GetCsvLines()));
            }
        }

        [TestMethod]
        [Ignore("Original")]
        public void LongPlay_HtmlParser_Strategy_Original_Test()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            LongPlayHtmlParserStrategy strategy = new LongPlayHtmlParserStrategy(logger, htmlGetter, recordProcessor);

            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [Ignore("Original")]
        public void VinylShop_HtmlParser_Strategy_Original_Test()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            VinylShopHtmlParserStrategy strategy = new VinylShopHtmlParserStrategy(logger, htmlGetter, recordProcessor);

            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
