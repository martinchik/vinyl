using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vinyl.RecordProcessingJob.Job;

namespace Vinyl.RecordProcessingJob.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly ProcessingJob _job;

        public StatusController(ProcessingJob job)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
        }

        [HttpGet]
        public dynamic GetStatus()
        {
            return new
            {
                Name = _job.JobName,
                Status = _job.IsRunning ? "running" : "suspend",
                LastStartDate = _job.LastStart,
                LastFinishDate = _job.LastFinish,
                Result = _job.Result
            };
        }

        [HttpGet("health")]
        public string GetHealth()
        {
            return "OK";
        }

        [HttpGet("restart")]
        public string Restart()
        {
            _job.Stop();
            _job.Start();

            return "Restarted";
        }
    }
}
