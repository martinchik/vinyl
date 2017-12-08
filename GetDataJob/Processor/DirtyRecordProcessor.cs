using GetDataJob.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GetDataJob.Processor
{
    public class DirtyRecordProcessor : IDirtyRecordProcessor
    {
        private readonly ILogger _logger;

        public DirtyRecordProcessor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        List<DirtyRecord> data = new List<DirtyRecord>();
        public void AddRecord(DirtyRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            data.Add(record);
        }
    }
}
