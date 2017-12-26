using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class SearchItemRepository : BaseRepositoryTemplate<SearchItem>
    {
        internal SearchItemRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.SearchItem, logger)
        {
        }        

        public SearchItem GetBy(Guid recordId)
        {
            if (recordId == Guid.Empty)
                return null;

            return Context.SearchItem.SingleOrDefault(t => t.RecordId == recordId);
        }
    }
}
