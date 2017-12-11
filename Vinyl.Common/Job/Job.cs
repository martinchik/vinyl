using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Common.Helpers;

namespace Vinyl.Common.Job
{
    public abstract class Job
    {
        protected readonly ILogger Logger;
        private readonly CancellationTokenSource _token;
        private Task _workingTask;
        private readonly TimeSpan? _executionTimeout;

        protected Job(ILogger logger, string jobName, TimeSpan? executionTimeout)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            JobName = jobName;
            _executionTimeout = executionTimeout;

            _token = new CancellationTokenSource();
        }

        public string JobName { get; }
        public bool IsRunning { get; private set; }
        public DateTime? LastStart { get; private set; }
        public DateTime? LastFinish { get; private set; }
        public string Result { get; protected set; }

        public void Start()
        {
            Logger.LogInformation($"{JobName}. Job working thread is started.");
            _workingTask = Task.Run(() => StartInternalAsync(_token.Token), _token.Token)
                               .ContinueWith(t => Logger.LogInformation($"{JobName}. Job working thread has been finished."), TaskContinuationOptions.ExecuteSynchronously);
        }

        private async Task StartInternalAsync(CancellationToken token)
        {
            //using (new JobMetric(JobName, true, token))
            {
                await Start(token);
            }
        }

        protected virtual async Task Start(CancellationToken token)
        {
            await TriggerExecution(token);
        }

        protected async Task TriggerExecution(CancellationToken token)
        {
            IsRunning = true;
            LastStart = DateTime.UtcNow;
            try
            {
                if (!_executionTimeout.HasValue)
                {
                    await ExecuteAsync(token);
                }
                else
                {
                    await ExecuteWithTimeoutAsync(_executionTimeout.Value, token);
                }
            }
            finally
            {
                IsRunning = false;
                LastFinish = DateTime.UtcNow;
            }
        }

        private async Task ExecuteWithTimeoutAsync(TimeSpan timeout, CancellationToken token)
        {
            await AsyncHelper.CallWithTimeoutAsync(() => ExecuteAsync(token).Wait(token), timeout);
        }

        protected internal abstract Task ExecuteAsync(CancellationToken token);

        public void Stop()
        {
            Logger.LogInformation($"{JobName}. Cancellation has been requested.");
            _token.Cancel();
            _workingTask.Wait(1000);
            if (_workingTask.IsCompleted)
            {
                _workingTask.Dispose();
            }
        }
    }
}
