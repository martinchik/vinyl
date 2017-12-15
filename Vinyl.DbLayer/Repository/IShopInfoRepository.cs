using System;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public interface IShopInfoRepository
    {
        void Delete(Guid id);
        ShopInfo Get(Guid id);
        ShopInfo Get(string title);
        IQueryable<ShopInfo> GetAll();
        void Save(ShopInfo item);
    }
}