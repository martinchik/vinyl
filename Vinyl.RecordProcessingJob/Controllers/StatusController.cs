using Microsoft.AspNetCore.Mvc;
using System;
using Vinyl.RecordProcessingJob.Job;

namespace Vinyl.RecordProcessingJob.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly ProcessingJob _job;
        private readonly AdditionalInfoSearchJob _searchJob;

        public StatusController(ProcessingJob job, AdditionalInfoSearchJob searchJob)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _searchJob = searchJob ?? throw new ArgumentNullException(nameof(searchJob));
        }

        [HttpGet]
        public dynamic[] GetStatus()
        {
            return new[] { new
            {
                Name = _job.JobName,
                Status = _job.IsRunning ? "running" : "suspend",
                LastStartDate = _job.LastStart,
                LastFinishDate = _job.LastFinish,
                Result = _job.Result
            }, new
            {
                Name = _searchJob.JobName,
                Status = _searchJob.IsRunning ? "running" : "suspend",
                LastStartDate = _searchJob.LastStart,
                LastFinishDate = _searchJob.LastFinish,
                Result = _searchJob.Result
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
            _searchJob.Stop();
            
            _job.Stop();
            _job.Start();

            _searchJob.Start();

            return "Restarted";
        }
    }
}
