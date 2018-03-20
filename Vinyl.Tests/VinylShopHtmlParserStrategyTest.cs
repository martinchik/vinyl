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
    public class VinylShopHtmlParserStrategyTest
    {
        [Test]
        public void VinylShopHtmlParserStrategyTest_Test()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var htmlLoader = new HtmlDataGetter(Substitute.For<ILogger<HtmlDataGetter>>());
            var strategy = new VinylShopHtmlParserStrategy(Substitute.For<ILogger<VinylShopHtmlParserStrategy>>(), htmlLoader);

            strategy.Initialize("http://www.vinylshop.by/products/page/{0}/");

            var records = strategy.Parse(CancellationToken.None).ToArray();
        }
    }
}
