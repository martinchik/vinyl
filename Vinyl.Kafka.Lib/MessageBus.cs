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
        private readonly Producer<Null, string> _producer;
        private Consumer<Null, string> _consumer;

        private readonly IDictionary<string, object> _producerConfig;
        private readonly IDictionary<string, object> _consumerConfig;

        public MessageBus() : this("localhost") { }

        public MessageBus(string host)
        {
            _producerConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", host }
            };
            _consumerConfig = new Dictionary<string, object>
            {
                { "group.id", "vinyl-group"},
                { "bootstrap.servers", host }
            };

            _producer = new Producer<Null, string>(_producerConfig, null, new StringSerializer(Encoding.UTF8));
        }

        public Task SendMessage<T>(string topic, T @object) where T : class
        {
            var msg = Newtonsoft.Json.JsonConvert.SerializeObject(@object);
#if DEBUG
            Console.WriteLine($"Sent msg:{msg}");
#endif
            return _producer.ProduceAsync(topic, null, msg);
        }        

        public void SubscribeOnTopic<T>(string topic, Action<T> action, CancellationToken cancellationToken) where T : class
        {
            var msgBus = new MessageBus();
            using (msgBus._consumer = new Consumer<Null, string>(_consumerConfig, null, new StringDeserializer(Encoding.UTF8)))
            {
                msgBus._consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, -1) });

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    Message<Null, string> msg;
                    if (msgBus._consumer.Consume(out msg, TimeSpan.FromMilliseconds(10)))
                    {
#if DEBUG
                        Console.WriteLine($"Recieved msg:{msg.Value}");
#endif
                        action(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg.Value));
                    }
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
