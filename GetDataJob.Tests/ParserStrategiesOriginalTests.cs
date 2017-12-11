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
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            var shops = new[] { Vinyl.Metadata.Test.TestShops.GetLongPlayShop() };
            IParserStrategy strategy = (new ShopStrategiesService(logger, htmlGetter, recordProcessor)).GetStrategiesForRun(shops).FirstOrDefault();
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
            //strategy.Initialize();
            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();
        }

        [TestMethod]
        [Ignore("Original")]
        public void VinylShop_Excel_Parser_Strategy_Original_Test()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            VinylShopExcelParserStrategy strategy = new VinylShopExcelParserStrategy(logger, htmlGetter, recordProcessor);
            strategy.Initialize("http://www.vinylshop.by/2015/10/%D1%81%D0%BE%D0%B2%D1%80%D0%B5%D0%BC%D0%B5%D0%BD%D0%BD%D1%8B%D0%B5-%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D1%80%D0%BE%D1%81%D1%81%D0%B8%D0%B9%D1%81%D0%BA%D0%B8%D0%B5-%D0%B8%D1%81/");
            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();

            SaveResultsTo(@"C:\test\allData_2.csv", recordProcessor);

            strategy.Initialize("http://www.vinylshop.by/2015/10/%D1%81%D0%BE%D0%B2%D1%80%D0%B5%D0%BC%D0%B5%D0%BD%D0%BD%D1%8B%D0%B5-%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D0%B7%D0%B0%D0%BF%D0%B0%D0%B4/");
            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();

            SaveResultsTo(@"C:\test\allData_3.csv", recordProcessor);
        }

        [TestMethod]
        [Ignore("Original")]
        public void VinylShopMM_Excel_Parser_Strategy_Original_Test()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            VinylShopMMExcelParserStrategy strategy = new VinylShopMMExcelParserStrategy(logger, htmlGetter, recordProcessor);
            strategy.Initialize("http://www.vinylshop.by/2015/07/%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D0%BC%D0%B8%D1%80%D1%83%D0%BC%D0%B8%D1%80/",
                refLinkText:"Скачать каталог");
            strategy.Run(CancellationToken.None).GetAwaiter().GetResult();

            SaveResultsTo(@"C:\test\allData_4.csv", recordProcessor);
        }
    }
}
