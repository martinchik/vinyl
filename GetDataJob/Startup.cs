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
using Vinyl.GetDataJob.Job;
using Vinyl.GetDataJob.Parsers;
using Vinyl.GetDataJob.Processor;
using Vinyl.GetDataJob.Data;

namespace Vinyl.GetDataJob
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

            services.AddSingleton<IHtmlDataGetter, HtmlDataGetter>();
            services.AddTransient<IDirtyRecordProcessor, DirtyRecordProcessor>();
            services.AddSingleton<IShopInfoService, ShopInfoService>();
            services.AddSingleton<IShopStrategiesService, ShopStrategiesService>();
            services.AddSingleton<ParsingJob>();

            services.AddMvc();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, 
            ParsingJob job)
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
        }
    }
}
