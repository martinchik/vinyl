using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vinyl.Common;

namespace Vinyl.DbLayer
{
    public static class DatabaseServiceRegistrator
    {
        internal static string GetConnectionString(IConfiguration configuration)
        {
            // Use a PostgreSQL database
            var connectionString = EnvironmentVariable.CONNECTION_STRING;

            if (string.IsNullOrEmpty(connectionString))
                connectionString = configuration.GetConnectionString("DefaultConnection");

            return connectionString;
        }

        internal static VinylShopContext CreateContext(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VinylShopContext>();
            optionsBuilder.UseNpgsql(GetConnectionString(configuration));

            return new VinylShopContext(optionsBuilder.Options);            
        }

        public static void Register(IConfiguration configuration, IServiceCollection services)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<VinylShopContext>(options => options
                    .UseNpgsql(GetConnectionString(configuration), b => b.MigrationsAssembly("Vinyl.DbLayer"))
                );

            services.AddTransient<IMetadataRepositoriesFactory, MetadataRepositoriesFactory>();
        }

        public static async Task MigrateDataBase(IConfiguration configuration)
        { 
            try
            {
                using (var ctx = CreateContext(configuration))
                {
                    await ctx.Database.MigrateAsync();

                    var mi = new MetadataInitializer();
                    if (EnvironmentVariable.CLEAR_DB)
                        mi.ClearData(ctx);

                    await ctx.RunScriptFromResources("Vinyl.DbLayer.Scripts.Support_FTS.sql");

                    mi.Initialize(ctx)
                        .RestartParsing(ctx);
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Trace.WriteLine("Error during update database. Exception:" + exc.ToString());
                throw;
            }
        }
    }
}
