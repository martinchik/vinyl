using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class ShopParseStrategyInfoRepository : BaseRepositoryTemplate<ShopParseStrategyInfo>
    {
        internal ShopParseStrategyInfoRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.ShopParseStrategyInfo, logger)
        {
        }        

        public IQueryable<ShopParseStrategyInfo> GetBy(Guid shopId)
        {
            if (shopId == Guid.Empty)
                return null;

            return Context.ShopParseStrategyInfo.Where(t => t.ShopId == shopId).Include(_ => _.Shop).AsQueryable();
        }

        public IQueryable<ShopParseStrategyInfo> GetAllWithShops()
        {
            return Context.ShopParseStrategyInfo.Include(_ => _.Shop).AsQueryable();
        }

        public void UpdateLastProcessedCount(Guid id, int count)
        {
            var strategy = Get(id);
            if (strategy == null)
                throw new KeyNotFoundException($"Strategy did not find with id={id}");

            strategy.LastProcessedCount = count;
            strategy.ProcessedAt = DateTime.UtcNow;
            strategy.UpdatedAt = DateTime.UtcNow;
        }

        public IEnumerable<ShopParseStrategyInfo> Get(Guid[] strategyIds)
        {
            if (strategyIds?.Any() != true)
                return Enumerable.Empty<ShopParseStrategyInfo>();

            return Context.ShopParseStrategyInfo.Where(_ => strategyIds.Contains(_.Id)).AsEnumerable();
        }
    }
}
