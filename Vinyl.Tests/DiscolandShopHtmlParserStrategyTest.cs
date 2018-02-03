using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vinyl.Common;
using Vinyl.ParsingJob.Parsers.HtmlParsers;

namespace Vinyl.Tests
{
    [TestFixture]
    public class DiscolandShopHtmlParserStrategyTest
    {
        [Test]
        public void DiscolandShopHtmlParserStrategy_Test()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var htmlLoader = new HtmlDataGetter(Substitute.For<ILogger<HtmlDataGetter>>());
            var strategy = new DiscolandShopHtmlParserStrategy(Substitute.For<ILogger<VinplazaShopHtmlParserStrategy>>(), htmlLoader);

            strategy.Initialize();

            var records = strategy.Parse(CancellationToken.None).ToArray();
        }
    }
}
