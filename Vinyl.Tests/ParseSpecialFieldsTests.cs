using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Common;

namespace Vinyl.ParsingJob.Tests
{
    [TestFixture]
    public class ParseSpecialFieldsTests
    {
        [TestCase("Ravi Shankar", "Ravi Shankar")]
        public void ParseRecordName_Test(string recordFrom, string recordTo)
        {
            var res = ParseSpecialFields.ParseRecordName(recordFrom);

            res.Should().Be(recordTo);
        }

        [TestCase("78.00", 78.0)]
        [TestCase("78.01", 78.01)]
        public void ParseRecordName_Test(string valueFrom, decimal? valueTo)
        {
            var res = ParseSpecialFields.ParsePrice(valueFrom, "byn");

            res.price.Should().Be(valueTo);
        }
    }
}
