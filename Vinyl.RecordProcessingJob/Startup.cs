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
using Vinyl.Common;
using Vinyl.Common.Helpers;
using Vinyl.Kafka;
using Vinyl.Kafka.Lib;
using Vinyl.RecordProcessingJob.Data;
using Vinyl.RecordProcessingJob.Job;
using Vinyl.RecordProcessingJob.Processor;

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
            services.AddTransient<IMessageConsumer>(_ => new KafkaConsumer(KafkaConstants.DirtyRecordTopicNameCmd, KafkaConstants.KafkaHostAddress));
            services.AddTransient<IDirtyRecordImportProcessor, DirtyRecordImportProcessor>();
            services.AddTransient<IRecordService, RecordService>();
            services.AddTransient<IAdditionalInfoSearchEngine, AdditionalInfoSearchEngine>();
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            applicationLifetime.ApplicationStopping.Register(() =>
            {
                job.Stop();
            });

            job.Start();            

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/" + SwaggerHelper.GetSwaggerPrefix() + "swagger/v1/swagger.json", "Record Processing Job API");
            });
        }
    }
}
