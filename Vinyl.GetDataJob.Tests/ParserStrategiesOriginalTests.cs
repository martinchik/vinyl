using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.GetDataJob.Data;
using Vinyl.GetDataJob.Parsers;
using Vinyl.GetDataJob.Parsers.HtmlParsers;
using Vinyl.GetDataJob.Processor;

namespace Vinyl.GetDataJob.Tests
{
    [TestClass]
    public class ParserStrategiesOriginalTests
    {       
        private void SaveResultsTo(string fileName, IDirtyRecordProcessor recordProcessor)
        {
            string[] header = new[] { "Parser;Album;Artist;Title;Year;State;Price;Info;Url" };
            if (File.Exists(fileName))
                File.Delete(fileName);

            File.WriteAllLines(fileName, header.Concat(recordProcessor.GetCsvLines()));
        }

        [TestMethod]
        [Ignore("Original")]
        public void LongPlay_HtmlParser_Strategy_Original_Test()
        {
            var htmlGetter = new HtmlDataGetter(Substitute.For<ILogger<HtmlDataGetter>>());
            var recordProcessor = new DirtyRecordProcessor(Substitute.For<ILogger<DirtyRecordProcessor>>());

            var shops = new[] { Vinyl.Metadata.Test.TestShops.GetLongPlayShop() };
            IParserStrategy strategy = (new ShopStrategiesService(Substitute.For<ILogger<ShopStrategiesService>>(), htmlGetter, recordProcessor)).GetStrategiesForRun(shops).FirstOrDefault();
            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();

            SaveResultsTo(strategy.GetType().Name + ".csv", recordProcessor);
        }        
    }
}
