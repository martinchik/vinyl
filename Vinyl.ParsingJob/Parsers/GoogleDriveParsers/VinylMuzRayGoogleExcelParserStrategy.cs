using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.GoogleDriveParsers
{
    public class VinylMuzRayGoogleExcelParserStrategy : GoogleBaseExcelParserStrategy
    {
        public VinylMuzRayGoogleExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        protected override string Name => "VinylMuzRayShopExcel";

        protected override IEnumerable<DirtyRecord> ParseDataFromDataSet(DataSet dataSet)
        {
            if (dataSet?.Tables?.Count > 0)
            {
                var table = dataSet.Tables[0];
                if (table?.Rows?.Count > 6)
                {
                    for (int i = 6; i < table.Rows.Count; i++)
                    {
                        var row = table.Rows[i].ItemArray;
                        StringBuilder sb = new StringBuilder();
                        DirtyRecord record = new DirtyRecord();
                        record.Title = row[1].ToString();
                        var artAndalb = ExtractFromTitle(record.Title);

                        record.Artist = artAndalb.artist;
                        record.Album = artAndalb.album;
                        record.YearRecorded = row[2].ToString();
                        record.Year = row[3].ToString();
                        record.Label = row[10].ToString();
                        record.Country = row[11].ToString();
                        record.Style = row[18].ToString();
                        record.CountInPack = row[15].ToString();
                        record.View = row[16].ToString();

                        AddLineIfExist(sb, "Новинка", row[0].ToString());
                        AddLineIfExist(sb, "Формат", row[5].ToString());
                        AddLineIfExist(sb, "Издание", row[12].ToString());
                        AddLineIfExist(sb, "Мастеринг", row[13].ToString());
                        AddLineIfExist(sb, "Серия", row[14].ToString());
                        AddLineIfExist(sb, "Жанр", row[17].ToString());

                        record.Info = sb.ToString();

                        record.Price = row[19].ToString();

                        yield return record;
                    }
                }
            }
        }

        private void AddLineIfExist(StringBuilder sb, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                sb.Append(name).Append(": ").Append(value.Trim()).AppendLine();
        }
             

        private (string artist, string album) ExtractFromTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                var fromInd = title.IndexOf(",");
                if (fromInd < 0)
                    fromInd = title.IndexOf("'");

                if (fromInd > 0)
                {
                    string artist = title.Substring(0, fromInd).Trim();
                    string album = title.Substring(fromInd + 1, title.Length - fromInd - 1).Replace("'", "").Trim();

                    return (artist, album);
                }
            }
            return (string.Empty, title);
        }
    }
}
