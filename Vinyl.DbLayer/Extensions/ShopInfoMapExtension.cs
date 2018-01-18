namespace Vinyl.DbLayer
{
    public static class ShopInfoMapExtension
    {
        public static Vinyl.Metadata.ShopInfo ToMetaData(this Vinyl.DbLayer.Models.ShopInfo obj)
            => new Vinyl.Metadata.ShopInfo
            {
                Id = obj.Id,
                Name = obj.Title,
                Url = obj.Url,
                CountryCode = obj.CountryCode
            };

        public static Vinyl.DbLayer.Models.ShopInfo ToDbObject(this Vinyl.Metadata.ShopInfo obj)
            => new Vinyl.DbLayer.Models.ShopInfo
            {
                Id = obj.Id,
                Title = obj.Name,
                Url = obj.Url,
                CountryCode = obj.CountryCode
            };
    }
}
