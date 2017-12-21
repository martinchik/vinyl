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

        public Task<List<ShopInfo>> GetShops(CancellationToken token)
        {
            return Task.Run(() =>
            {
                using (var shopInfoRepository = _metadataFactory.CreateShopInfoRepository())
                {
                    return shopInfoRepository.GetAll().Select(_ => _.ToMetaData()).ToList();
                }
            }, token);
        }
    }
}
