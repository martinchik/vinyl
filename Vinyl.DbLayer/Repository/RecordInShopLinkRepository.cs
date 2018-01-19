using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class RecordInShopLinkRepository : BaseRepositoryTemplate<RecordInShopLink>
    {
        internal RecordInShopLinkRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.RecordInShopLink, logger)
        {
        }
        
        public IEnumerable<RecordInShopLink> FindBy(Guid recordId, Guid shopId, Guid strategyId)
        {            
            return Context.RecordInShopLink.Where(_ => 
                _.RecordId == recordId &&
                _.ShopId == shopId &&
                _.StrategyId == strategyId);
        }

        public IQueryable<RecordInShopLink> FindBy(Guid recordId, string countryCode)
        {
            return Context.RecordInShopLink.Where(_ =>
                _.RecordId == recordId &&
                _.Shop.CountryCode == countryCode).AsQueryable();
        }

        public IQueryable<RecordInShopLink> FindByWithStrategy(Guid recordId)
        {
            return Context.RecordInShopLink
                .Include(_ => _.Strategy)
                .Where(_ => _.RecordId == recordId)
                .AsQueryable();
        }
    }
}
