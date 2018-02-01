using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Common.Helpers;
using Vinyl.Kafka;
using Vinyl.Kafka.Lib;
using Vinyl.Metadata;
using Vinyl.RecordProcessingJob.Data;
using Vinyl.RecordProcessingJob.Job;
using Vinyl.RecordProcessingJob.Processor;
using Vinyl.RecordProcessingJob.SearchEngine;

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
            services.AddMemoryCache();
            
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((version, apiDescription) => { apiDescription.RelativePath = SwaggerHelper.ApiChangeRelativePath(apiDescription.RelativePath); return true; });
                c.SwaggerDoc("v1", new Info { Title = "Record Processing Job API", Version = "v1", TermsOfService = "None", Description = "Functions for monitoring Record Processing Job " });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");
                c.IncludeXmlComments(xmlPath);
            });

            DbLayer.DatabaseServiceRegistrator.Register(Configuration, services);
            
            services.AddTransient<ICurrencyConverter, CurrencyConverter>();
            services.AddTransient<IHtmlDataGetter, HtmlDataGetter>();
            services.AddTransient<IMessageConsumer<DirtyRecord>>(_ => new KafkaConsumer<DirtyRecord>(EnvironmentVariable.KAFKA_DIRTY_RECORDS_TOPIC, EnvironmentVariable.KAFKA_CONNECT));
            services.AddTransient<IMessageProducer<FindInfosRecord>>(_ => new KafkaProducer<FindInfosRecord>(EnvironmentVariable.KAFKA_FIND_INFO_RECORDS_TOPIC, EnvironmentVariable.KAFKA_CONNECT));
            services.AddTransient<IMessageConsumer<FindInfosRecord>>(_ => new KafkaConsumer<FindInfosRecord>(EnvironmentVariable.KAFKA_FIND_INFO_RECORDS_TOPIC, EnvironmentVariable.KAFKA_CONNECT));
            services.AddTransient<IDirtyRecordImportProcessor, DirtyRecordImportProcessor>();
            services.AddTransient<IRecordService, RecordService>();
            services.AddTransient<IAdditionalInfoSearchEngine, AdditionalInfoSearchEngine>();
            services.AddTransient<IDiscogsSearchEngine, DiscogsSearchEngine>();
            
            services.AddSingleton<ProcessingJob>();
            services.AddSingleton<AdditionalInfoSearchJob>();
            services.AddSingleton<AliveValidationJob>();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, 
            ProcessingJob job, AdditionalInfoSearchJob searchJob, AliveValidationJob aliveJob)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                job.Stop();
                searchJob.Stop();
                aliveJob.Stop();
            });

            Task.Delay(TimeSpan.FromSeconds(20)).Wait(); // wait while database will be loaded

            job.Start();
            searchJob.Start();
            aliveJob.Start();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/" + SwaggerHelper.GetSwaggerPrefix() + "swagger/v1/swagger.json", "Record Processing Job API");
            });
        }
    }
}
