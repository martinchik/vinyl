using Vinyl.Metadata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Kafka;

namespace Vinyl.ParsingJob.Processor
{
    public class DirtyRecordProcessor : IDirtyRecordProcessor
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;

        public DirtyRecordProcessor(ILogger<DirtyRecordProcessor> logger, IMessageBus messageBus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        private Dictionary<string, List<DirtyRecord>> _data = new Dictionary<string, List<DirtyRecord>>();

        public void AddRecord(string strategyName, DirtyRecord record)
        {
            if (record == null ||
                string.IsNullOrEmpty(record.Title) ||
                string.IsNullOrEmpty(record.Artist))
                return;

            _messageBus.SendMessage(KafkaConstants.DirtyRecordTopicNameCmd, record);

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
