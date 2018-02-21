using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Kafka;
using Vinyl.Metadata;
using Vinyl.ParsingJob.Parsers.GoogleDriveParsers;
using Vinyl.ParsingJob.Parsers.HtmlParsers;
using Vinyl.ParsingJob.Processor;

namespace Vinyl.Tests
{
    [TestFixture]
    public class VinylMuzRayGoogleExcelParserStrategyTest
    {
        [Test]
        public void VinylMuzRayGoogleExcelParserStrategy_Test()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var htmlLoader = new HtmlDataGetter(Substitute.For<ILogger<HtmlDataGetter>>());
            var strategy = new VinylMuzRayGoogleExcelParserStrategy(Substitute.For<ILogger<VinplazaShopHtmlParserStrategy>>(), htmlLoader);
            var messageBus = Substitute.For<IMessageProducer<DirtyRecord>>();
            var strategyInfo = Substitute.For<ShopParseStrategyInfo>();
            var counter = 0;

            messageBus.SendMessage(Arg.Any<DirtyRecord>()).Returns(obj =>
            {
                Interlocked.Increment(ref counter);
                return Task.FromResult("ok");
            });

            var processor = new DirtyRecordExportProcessor(Substitute.For<ILogger<DirtyRecordExportProcessor>>(), messageBus);
            strategy.Initialize("1rF7EpvUTVqqY8UoXK6FQERCQzTZYUfxH");

            var records = strategy.Parse(CancellationToken.None).ToArray();
            Array.ForEach(records, _ => processor.AddRecord(strategyInfo, _));

            records.Count().Should().Be(counter);
        }
    }
}
