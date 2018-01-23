using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka
{
    public interface IMessageConsumer<T> : IDisposable
    {
        void SubscribeOnTopic(Action<T, string> action, Action keepAliveAction, CancellationToken cancellationToken);
    }

    public interface IMessageProducer<T> : IDisposable
    {
        Task<string> SendMessage(T @object);
    }
}