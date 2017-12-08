using GetDataJob.Parsers;
using GetDataJob.Parsers.HtmlParsers;
using GetDataJob.Processor;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GetDataJob.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var logger = Substitute.For<ILogger>();
            var htmlGetter = new HtmlDataGetter(logger);
            var recordProcessor = new DirtyRecordProcessor(logger);

            //LongPlayHtmlParserStrategy strategy = new LongPlayHtmlParserStrategy(logger, htmlGetter, recordProcessor);

            //strategy.Run().GetAwaiter().GetResult();
        }
    }
}
