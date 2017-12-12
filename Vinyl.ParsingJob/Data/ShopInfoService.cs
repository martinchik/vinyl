using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Metadata;
using Vinyl.Metadata.Test;

namespace Vinyl.ParsingJob.Data
{
    public class ShopInfoService : IShopInfoService
    {
        public async Task<IList<ShopInfo>> GetShops(CancellationToken token)
        {
            return await Task.FromResult(TestShops.GetAll().ToList());
        }
    }
}
