using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.DbLayer.Models
{
    public class StatisticsItem
    {
        public long CountShops { get; set; }
        public long CountStrategies { get; set; }
        public long CountParsedRecords { get; set; }
        public long CountRecordItems { get; set; }
        public long CountSearchItems { get; set; }
    }
}
