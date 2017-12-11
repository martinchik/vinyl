using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.Common.Job
{
    public abstract class RepeatableJob : Job
    {
        private readonly IEnumerable<DateTime> _waitingInterval;

        protected RepeatableJob(ILogger logger, string jobname, IEnumerable<DateTime> waitingInterval, TimeSpan? executionTimeout)
            : base(logger, jobname, executionTimeout)
        {
            _waitingInterval = waitingInterval ?? throw new ArgumentNullException(nameof(waitingInterval));
        }

        protected sealed override async Task Start(CancellationToken token)
        {
            foreach (var time in _waitingInterval)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    var now = DateTime.UtcNow;
                    if (time > now & time - now > TimeSpan.FromSeconds(1))
                    {
                        Logger.LogInformation($"{JobName}. Next round will be at {time} in UTC. Sleep for {time - now}.");
                        await Task.Delay(time - now, token);
                    }

                    if (!token.IsCancellationRequested)
                    {
                        //using (new JobMetric(JobName + ".repeatable", _reportJobEvents, token))
                        {
                            Logger.LogInformation($"{JobName}. Repeat started.");
                            await TriggerExecution(token);
                            Logger.LogInformation($"{JobName}. Repeat finished.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.LogInformation($"{JobName}. Job token has been cancelled.");
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, $"{JobName}. Job failed");
                }
            }
        }
    }
}
