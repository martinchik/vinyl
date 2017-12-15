using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Vinyl.DbLayer.Repository;

namespace Vinyl.DbLayer
{
    public static class DatabaseServiceRegistrator
    {
        public static void Register(IServiceCollection services)
        {
            // Use a PostgreSQL database
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            services
                //.AddEntityFrameworkNpgsql()
                .AddDbContext<VinylShopContext>(options =>
                    options.UseNpgsql(
                        connectionString,
                        b => b.MigrationsAssembly("Vinyl.DbLayer")
                    )
                );

            services.AddTransient<IShopInfoRepository, ShopInfoRepository>();
        }
    }
}
