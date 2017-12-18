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
using Vinyl.ParsingJob.Job;
using Vinyl.ParsingJob.Parsers;
using Vinyl.ParsingJob.Processor;
using Vinyl.ParsingJob.Data;
using Vinyl.Kafka.Lib;
using Vinyl.Kafka;
using System.Net;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace Vinyl.ParsingJob
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
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            string myIP = Dns.GetHostAddresses(hostName).FirstOrDefault().ToString();
            string testHost = $"{myIP}:{KafkaConstants.KafkaHostPort}";

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Parsing Job API", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, $"{PlatformServices.Default.Application.ApplicationName}.xml");
                c.IncludeXmlComments(xmlPath);
            });

            DbLayer.DatabaseServiceRegistrator.Register(Configuration, services);

            services.AddTransient<IHtmlDataGetter, HtmlDataGetter>();
            services.AddTransient<IDirtyRecordProcessor, DirtyRecordProcessor>();
            services.AddTransient<IMessageProducer>(_ => new KafkaProducer(KafkaConstants.DirtyRecordTopicNameCmd, KafkaConstants.KafkaHostAddress));
            services.AddTransient<IShopInfoService, ShopInfoService>();
            services.AddTransient<IShopStrategiesService, ShopStrategiesService>();
            services.AddSingleton<ParsingRepeatableJob>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, ParsingRepeatableJob job)
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
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parsing Job API"); });
        }
    }
}
