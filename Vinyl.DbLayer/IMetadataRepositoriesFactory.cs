using Vinyl.DbLayer.Repository;

namespace Vinyl.DbLayer
{
    public interface IMetadataRepositoriesFactory
    {
        ShopInfoRepository CreateShopInfoRepository();
    }
}