﻿using ExcelDataReader;
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
    public class VinylShopExcelParserStrategy : BaseExcelParserStrategy
    {
        public VinylShopExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor)
            : base(logger, htmlDataGetter, recordProcessor)
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
                    record.Price = row[4].ToString();

                    yield return record;
                }
            }
        }        
    }
}
