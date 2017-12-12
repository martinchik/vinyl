using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka.Lib
{
    public class MessageBus : IMessageBus
    {
        private Producer<Null, string> _producer;
        private Consumer<Null, string> _consumer;
        private readonly string _topic;

        private readonly IDictionary<string, object> _producerConfig;
        private readonly IDictionary<string, object> _consumerConfig;
        
        public MessageBus(string topic, string host)
        {
            _producerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", host }
            };
            _consumerConfig = new Dictionary<string, object>
            {
                { "group.id", "custom-group"},
                { "bootstrap.servers", host }
            };
            _topic = topic;            
        }

        public Task SendMessage<T>(T @object) where T : class
        {
            if (_producer == null)
                _producer = new Producer<Null, string>(_producerConfig, null, new StringSerializer(Encoding.UTF8));

            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(@object);
#if DEBUG
            Console.WriteLine($"Sent msg:{msg}");
#endif
            return _producer.ProduceAsync(_topic, null, msg);
        }        

        public void SubscribeOnTopic<T>(Action<T> action, CancellationToken cancellationToken) where T : class
        {
            if (_consumer != null)
                throw new Exception("Already subscribed on topic " + _topic);

            _consumer = new Consumer<Null, string>(_consumerConfig, null, new StringDeserializer(Encoding.UTF8));            
            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_topic, 0, -1) });

            while (!cancellationToken.IsCancellationRequested)
            {
                Message<Null, string> msg;
                if (_consumer.Consume(out msg, TimeSpan.FromMilliseconds(10)))
                {
#if DEBUG
                    Console.WriteLine($"Recieved msg:{msg.Value}");
#endif
                    action(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg.Value));
                }
            }
        }        

        public void Dispose()
        {
            _producer?.Dispose();
            _consumer?.Dispose();
        }
    }
}
