using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka.Lib
{
    public class KafkaConsumer : IMessageConsumer
    {
        private readonly Consumer<Null, string> _consumer;
        private readonly string _topic;
        private readonly IDictionary<string, object> _consumerConfig;
        
        public KafkaConsumer(string topic, string host)
        {            
            _consumerConfig = new Dictionary<string, object>
            {
                { "group.id", "vinyl-group"},
                { "bootstrap.servers", host }
            };
            _topic = topic;
            _consumer = new Consumer<Null, string>(_consumerConfig, null, new StringDeserializer(Encoding.UTF8));
            _consumer.OnError += (sender, err) =>
            {
                Console.WriteLine($"code:{err.Code}; reason:{err.Reason}");
            };
            _consumer.OnLog += (sender, logMsg) =>
            {
                Console.WriteLine($"{logMsg.Name}: {logMsg.Message}");
            };
            _consumer.OnStatistics += (sender, stat) =>
            {
                Console.WriteLine($"Statistics: {stat}");
            };
        }
        
        public void SubscribeOnTopic<T>(Action<T, string> action, CancellationToken cancellationToken) where T : class
        {
            if (_consumer.Assignment?.Count > 0)
                throw new Exception("Already subscribed on topic " + _topic);

            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_topic, 0, -1) });

            while (!cancellationToken.IsCancellationRequested)
            {
                Message<Null, string> msg;
                if (_consumer.Consume(out msg, TimeSpan.FromMilliseconds(10)))
                {
                    action(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg.Value), $"Recieved message on Partition: {msg.Partition} with Offset: {msg.Offset}. Content:{msg.Value}");
                }
            }
        }        

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }
}
