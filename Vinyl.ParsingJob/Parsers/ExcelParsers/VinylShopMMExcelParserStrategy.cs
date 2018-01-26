using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.ExcelParsers
{
    public class VinylShopMMExcelParserStrategy : BaseExcelParserStrategy
    {
        public VinylShopMMExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }        

        protected override string Name => "VinylMMShopExcel";
        
        protected override IEnumerable<DirtyRecord> ParseDataFromDataSet(DataSet dataSet)
        {
            if (dataSet?.Tables?.Count > 0)
            {
                var table = dataSet.Tables[0];
                for(int i=3; i < table.Rows.Count;i++)
                {
                    var row = table.Rows[i].ItemArray;

                    DirtyRecord record = new DirtyRecord();
                    record.Artist = row[0].ToString();
                    record.Title = record.Album = row[1].ToString();
                    record.Barcode = row[4].ToString();
                    record.Year = row[7].ToString();
                    record.Price = row[9].ToString() + " р";
                    record.Style = row[6].ToString();
                    record.CountInPack = row[2].ToString();
                    record.View = row[3].ToString();
                    record.Label = row[5].ToString();
                    record.Info = row[8].ToString();

                    yield return record;
                }
            }
        }        
    }
}
