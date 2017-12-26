using Vinyl.DbLayer.Repository;

namespace Vinyl.DbLayer
{
    public interface IMetadataRepositoriesFactory
    {
        ShopInfoRepository CreateShopInfoRepository();
        ShopParseStrategyInfoRepository CreateShopParseStrategyInfoRepository();
        RecordInfoRepository CreateRecordInfoRepository();
        RecordInShopLinkRepository CreateRecordInShopLinkRepository();
        SearchItemRepository CreateSearchItemRepository();
    }
}