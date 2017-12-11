using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Common.Helpers
{
    public static class AsyncHelper
    {
        public static async Task CallWithTimeoutAsync(Action action, TimeSpan timeout)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            var cts = new CancellationTokenSource();
            var backgroundTask = Task.Factory.StartNew(wrappedAction, cts.Token);
            await Task.WhenAny(backgroundTask, Task.Delay(timeout, cts.Token))
                       .ContinueWith(t =>
                       {
                           if (backgroundTask.IsFaulted && backgroundTask.Exception != null)
                           {
                               throw backgroundTask.Exception;
                           }

                           if (!backgroundTask.IsCompleted)
                           {
                               threadToKill.Abort();
                               cts.Cancel();
                               throw new TimeoutException("Timeout expired");
                           }
                       }, cts.Token);
        }
    }
}
