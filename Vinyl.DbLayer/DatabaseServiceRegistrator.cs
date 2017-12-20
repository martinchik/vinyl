using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vinyl.DbLayer
{
    public static class DatabaseServiceRegistrator
    {
        internal static string GetConnectionString(IConfiguration configuration)
        {
            // Use a PostgreSQL database
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

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
                .AddDbContext<VinylShopContext>(options =>
                    options.UseNpgsql(
                        GetConnectionString(configuration),
                        b => b.MigrationsAssembly("Vinyl.DbLayer")
                    )
                );

            services.AddTransient<IMetadataRepositoriesFactory, MetadataRepositoriesFactory>();

            try
            {
                using (var ctx = CreateContext(configuration))
                {
                    ctx.Database.Migrate();

                    new MetadataInitializer().Initialize(ctx);
                }
            }
            catch (Exception exc)
            {
                System.Diagnostics.Trace.WriteLine("Error during update database. Exception:" + exc.ToString());
                throw exc;
            }
        }
    }
}
