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
    }
}
