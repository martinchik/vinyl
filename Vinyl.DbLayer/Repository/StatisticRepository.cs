using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class StatisticRepository : BaseRepository
    {
        public StatisticRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            : base(context, logger)
        {
        }

        public async Task<StatisticsItem> GetStats()
        {
            var statistic = new StatisticsItem();

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"select 
(SELECT count(*) FROM ""ShopInfo"") as shops,
(SELECT count(*) FROM ""ShopParseStrategyInfo"") as strategies,
(SELECT sum(""LastProcessedCount"") FROM ""ShopParseStrategyInfo"") as parsed,
(SELECT count(*) FROM ""RecordInfo"") as records,
(SELECT count(*) FROM ""SearchItem"") as items";

                Context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    if (await result.ReadAsync())
                    {
                        statistic.CountShops = result.GetInt64(0);
                        statistic.CountStrategies = result.GetInt64(1);
                        statistic.CountParsedRecords = result.GetInt64(2);
                        statistic.CountRecordItems = result.GetInt64(3);
                        statistic.CountSearchItems = result.GetInt64(4);
                    }
                }
            }

            return statistic;
        }
    }
}
