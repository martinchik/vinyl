using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public class VinylShopExcelParserStrategy : BaseExcelParserStrategy
    {
        public VinylShopExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }        

        protected override string Name => "VinylShopExcel";
        
        protected override IEnumerable<DirtyRecord> ParseDataFromDataSet(DataSet dataSet)
        {
            if (dataSet?.Tables?.Count > 0)
            {
                var table = dataSet.Tables[0];
                for(int i=2; i < table.Rows.Count;i++)
                {
                    var row = table.Rows[i].ItemArray;

                    DirtyRecord record = new DirtyRecord();
                    record.Artist = row[0].ToString();
                    record.Title = record.Album = row[1].ToString();
                    record.Barcode = row[2].ToString();
                    record.Year = row[3].ToString();
                    record.Price = row[4].ToString() + " р";

                    yield return record;
                }
            }
        }        
    }
}
