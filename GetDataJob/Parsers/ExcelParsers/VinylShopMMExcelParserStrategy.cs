using ExcelDataReader;
using GetDataJob.Model;
using GetDataJob.Processor;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Parsers.HtmlParsers
{
    public class VinylShopMMExcelParserStrategy : BaseExcelParserStrategy
    {
        public VinylShopMMExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor)
            : base(logger, htmlDataGetter, recordProcessor)
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
                    record.Price = row[9].ToString();

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Concat("Count:", row[2].ToString()));
                    sb.AppendLine(string.Concat("View:", row[3].ToString()));
                    sb.AppendLine(string.Concat("Label:", row[5].ToString()));
                    sb.AppendLine(string.Concat("Style:", row[6].ToString()));
                    sb.AppendLine(string.Concat("Info:", row[8].ToString()));

                    record.Info = sb.ToString();
                    yield return record;
                }
            }
        }        
    }
}
