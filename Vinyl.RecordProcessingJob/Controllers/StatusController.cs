using Microsoft.AspNetCore.Mvc;
using System;
using Vinyl.Common.Job;
using Vinyl.RecordProcessingJob.Job;

namespace Vinyl.RecordProcessingJob.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly ProcessingJob _job;
        private readonly AdditionalInfoSearchJob _searchJob;
        private readonly AliveValidationJob _aliveJob;

        public StatusController(ProcessingJob job, AdditionalInfoSearchJob searchJob, AliveValidationJob aliveJob)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _searchJob = searchJob ?? throw new ArgumentNullException(nameof(searchJob));
            _aliveJob = aliveJob ?? throw new ArgumentNullException(nameof(aliveJob));
        }

        [HttpGet]
        public JobStatusInfo[] GetStatus()
        {
            return new[] { new JobStatusInfo
            {
                Name = _job.JobName,
                Status = _job.IsRunning ? "running" : "suspend",
                LastStartDate = _job.LastStart,
                LastFinishDate = _job.LastFinish,
                Result = _job.Result
            }, new JobStatusInfo
            {
                Name = _searchJob.JobName,
                Status = _searchJob.IsRunning ? "running" : "suspend",
                LastStartDate = _searchJob.LastStart,
                LastFinishDate = _searchJob.LastFinish,
                Result = _searchJob.Result
            }, new JobStatusInfo
            {
                Name = _aliveJob.JobName,
                Status = _aliveJob.IsRunning ? "running" : "suspend",
                LastStartDate = _aliveJob.LastStart,
                LastFinishDate = _aliveJob.LastFinish,
                Result = _aliveJob.Result
            }};
        }

        [HttpGet("health")]
        public string GetHealth()
        {
            return "OK";
        }

        [HttpGet("restart")]
        public string Restart()
        {
            _aliveJob.Stop();
            _searchJob.Stop();
            
            _job.Stop();
            _job.Start();

            _searchJob.Start();
            _aliveJob.Start();

            return "Restarted";
        }
    }
}
