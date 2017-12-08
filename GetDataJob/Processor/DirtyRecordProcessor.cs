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

        private Dictionary<string, List<DirtyRecord>> _data = new Dictionary<string, List<DirtyRecord>>();

        public void AddRecord(string strategyName, DirtyRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            List<DirtyRecord> list;
            if (!_data.TryGetValue(strategyName, out list))
            {
                list = new List<DirtyRecord>();
                _data.Add(strategyName, list);
            }
            list.Add(record);
        }

        public IEnumerable<string> GetCsvLines()
        {
            foreach (var pair in _data)
            {
                foreach (var rec in pair.Value)
                {
                    yield return string.Concat(
                        pair.Key.ToCsvValue(),
                        rec.Album.ToCsvValue(),
                        rec.Artist.ToCsvValue(),
                        rec.Title.ToCsvValue(),
                        rec.Year.ToCsvValue(),
                        rec.State.ToCsvValue(),
                        rec.Price.ToCsvValue(),
                        rec.Info.ToCsvValue(),
                        rec.Url.ToCsvValue()
                        );

                }
            }
        }

    }
}
