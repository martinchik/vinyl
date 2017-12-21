using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vinyl.RecordProcessingJob.Job;
using Vinyl.Kafka;
using Vinyl.Kafka.Lib;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Vinyl.RecordProcessingJob.Processor;
using Vinyl.RecordProcessingJob.Data;

namespace Vinyl.RecordProcessingJob
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Record Processing Job API", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");
                c.IncludeXmlComments(xmlPath);
            });

            DbLayer.DatabaseServiceRegistrator.Register(Configuration, services);

            services.AddTransient<IMessageConsumer>(_ => new KafkaConsumer(KafkaConstants.DirtyRecordTopicNameCmd, KafkaConstants.KafkaHostAddress));
            services.AddTransient<IDirtyRecordImportProcessor, DirtyRecordImportProcessor>();
            services.AddTransient<IRecordService, RecordService>();
            services.AddSingleton<ProcessingJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ProcessingJob job)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                job.Stop();
            });

            job.Start();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Record Processing Job API"); });
        }
    }
}
