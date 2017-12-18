using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class ShopInfoRepository : BaseRepository
    {
        internal ShopInfoRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, logger)
        {
        }

        public void Save(ShopInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new ArgumentNullException(nameof(item.Title));

            var other = Get(item.Title);
            if (other != null && other.Id != item.Id)
                throw new Exception($"Shop '{item.Title}' is already exist");

            if (item.Id == Guid.Empty)
                Context.ShopInfo.Add(item);
            else
                Context.ShopInfo.Update(item);

            Context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            Context.ShopInfo.Remove(entity);
            Context.SaveChanges();
        }

        public ShopInfo Get(Guid id)
        {
            if (id == Guid.Empty)
                return null;
            return Context.ShopInfo.Find(id);
        }

        public ShopInfo Get(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            return Context.ShopInfo.FirstOrDefault(t => string.Compare(t.Title, title, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IQueryable<ShopInfo> GetAll()
        {
            return Context.ShopInfo.AsQueryable();
        }
    }
}
