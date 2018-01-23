using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vinyl.Common.Job;
using Vinyl.ParsingJob.Job;

namespace Vinyl.ParsingJob.Controllers
{
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private readonly ParsingRepeatableJob _job;

        public StatusController(ParsingRepeatableJob job)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
        }

        [HttpGet]
        public JobStatusInfo GetStatus()
        {
            return new JobStatusInfo
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
