using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer.Repository;
using Vinyl.Metadata;
using Vinyl.Metadata.Test;

namespace Vinyl.ParsingJob.Data
{
    public class ShopInfoService : IShopInfoService
    {
        private readonly IShopInfoRepository _shopInfoRepository;

        public ShopInfoService(IShopInfoRepository shopInfoRepository)
        {
            _shopInfoRepository = shopInfoRepository ?? throw new ArgumentNullException(nameof(shopInfoRepository));
        }

        public async Task<IList<ShopInfo>> GetShops(CancellationToken token)
        {
            var res = _shopInfoRepository.GetAll().ToList();
            return await Task.FromResult(TestShops.GetAll().ToList());
        }
    }
}
