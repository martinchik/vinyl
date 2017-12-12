using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Kafka
{
    public interface IMessageBus : IDisposable
    {
        Task SendMessage<T>(T @object) where T : class;
        void SubscribeOnTopic<T>(Action<T> action, CancellationToken cancellationToken) where T : class;
    }
}