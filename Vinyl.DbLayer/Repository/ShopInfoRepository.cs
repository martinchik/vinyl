using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class ShopInfoRepository : BaseRepositoryTemplate<ShopInfo>
    {
        internal ShopInfoRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.ShopInfo, logger)
        {
        }

        public ShopInfo FindBy(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            return Context.ShopInfo.FirstOrDefault(t => string.Compare(t.Title, title, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IEnumerable<ShopInfo> Get(Guid[] shopIds)
        {
            if (shopIds?.Any() != true)
                return Enumerable.Empty<ShopInfo>();

            return Context.ShopInfo.Where(_ => shopIds.Contains( _.Id)).AsEnumerable();
        }
    }
}
