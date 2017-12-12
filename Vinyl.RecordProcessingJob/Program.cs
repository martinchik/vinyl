using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Vinyl.RecordProcessingJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConsole();
                    builder.AddFile("Logs/get-data-job-{Date}.log");
                })
                .UseStartup<Startup>()
                .Build();
    }
}
