using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class ShopParseStrategyInfoRepository : BaseRepository
    {
        internal ShopParseStrategyInfoRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, logger)
        {
        }

        public void Save(ShopParseStrategyInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var other = Get(item.Id);
            if (other != null && other.Id != item.Id)
                throw new Exception($"Shop '{item.Id}' is already exist");

            if (item.Id == Guid.Empty)
                Context.ShopParseStrategyInfo.Add(item);
            else
                Context.ShopParseStrategyInfo.Update(item);
        }

        public void Delete(Guid id)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            Context.ShopParseStrategyInfo.Remove(entity);
        }

        public ShopParseStrategyInfo Get(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return Context.ShopParseStrategyInfo.Find(id);
        }

        public IQueryable<ShopParseStrategyInfo> GetBy(Guid shopId)
        {
            if (shopId == Guid.Empty)
                return null;

            return Context.ShopParseStrategyInfo.Where(t => t.ShopId == shopId).Include(_ => _.Shop).AsQueryable();
        }

        public IQueryable<ShopParseStrategyInfo> GetAll()
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
    }
}
