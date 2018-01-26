using ExcelDataReader;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.GoogleDriveParsers
{
    public abstract class GoogleBaseExcelParserStrategy : GoogleDriveParserStrategy
    {
        public GoogleBaseExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        protected override IEnumerable<DirtyRecord> ParseFileFromGoogleDrive(string fileName)
        {
            return ParseDataFromDataSet(ReadExcelFile(fileName));
        }

        protected abstract IEnumerable<DirtyRecord> ParseDataFromDataSet(DataSet dataSet);

        private DataSet ReadExcelFile(string filePath)
        {
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return reader.AsDataSet();
                }
            }
        }
    }
}
