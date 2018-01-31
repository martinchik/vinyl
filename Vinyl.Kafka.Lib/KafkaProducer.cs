using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka.Lib
{
    public class KafkaProducer<T> : IMessageProducer<T>
         where T : class
    {
        private readonly Producer<Null, string> _producer;
        private readonly string _topic;
        private readonly IDictionary<string, object> _producerConfig;
        
        public KafkaProducer(string topic, string host)
        {
            _producerConfig = new Dictionary<string, object>
            {
                { "group.id", "vinylgroup"},
                { "bootstrap.servers", host }
            };

            _topic = topic;
            _producer = new Producer<Null, string>(_producerConfig, null, new StringSerializer(Encoding.UTF8));
            _producer.OnError += (sender, err) =>
            {
                Console.WriteLine($"code:{err.Code}; reason:{err.Reason}");
            };
            _producer.OnLog += (sender, logMsg) =>
            {
                Console.WriteLine($"{logMsg.Name}: {logMsg.Message}");
            };
            _producer.OnStatistics += (sender, stat) =>
            {
                Console.WriteLine($"Statistics: {stat}");
            };
        }

        public Task<string> SendMessage(T @object)
        {            
            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(@object);
            return _producer.ProduceAsync(_topic, null, msg).ContinueWith(_ =>
            {
                return $"Sent message on Partition: {_.Result.Partition} with Offset: {_.Result.Offset}. Content:{_.Result}";
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}
