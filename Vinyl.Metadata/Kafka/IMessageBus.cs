using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka
{
    public interface IMessageConsumer : IDisposable
    {
        void SubscribeOnTopic<T>(Action<T, string> action, CancellationToken cancellationToken) where T : class;
    }

    public interface IMessageProducer : IDisposable
    {
        Task<string> SendMessage<T>(T @object) where T : class;
    }
}