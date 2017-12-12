using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka
{
    public interface IMessageBus : IDisposable
    {
        Task SendMessage<T>(string topic, T @object) where T : class;
        void SubscribeOnTopic<T>(string topic, Action<T> action, CancellationToken cancellationToken) where T : class;
    }
}