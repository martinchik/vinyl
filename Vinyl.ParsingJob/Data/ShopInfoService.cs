using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Repository;
using Vinyl.Metadata;
using Vinyl.Metadata.Test;

namespace Vinyl.ParsingJob.Data
{
    public class ShopInfoService : IShopInfoService
    {
        private readonly IMetadataRepositoriesFactory _metadataFactory;

        public ShopInfoService(IMetadataRepositoriesFactory metadataFactory)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
        }

        public async Task<IList<ShopInfo>> GetShops(CancellationToken token)
        {
            using (var shopInfoRepository = _metadataFactory.CreateShopInfoRepository())
            {
                var res = shopInfoRepository.GetAll().ToList();
                return await Task.FromResult(TestShops.GetAll().ToList());
            }
        }
    }
}
