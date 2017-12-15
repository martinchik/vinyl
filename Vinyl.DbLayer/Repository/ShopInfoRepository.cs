using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class ShopInfoRepository : IShopInfoRepository
    {
        private readonly VinylShopContext _context;
        private readonly ILogger _logger;

        public ShopInfoRepository(VinylShopContext context, ILogger<ShopInfoRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                _context.ShopInfo.Add(item);
            else
                _context.ShopInfo.Update(item);

            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var entity = Get(id);
            if (entity == null)
                return;

            _context.ShopInfo.Remove(entity);
            _context.SaveChanges();
        }

        public ShopInfo Get(Guid id)
        {
            if (id == Guid.Empty)
                return null;
            return _context.ShopInfo.Find(id);
        }

        public ShopInfo Get(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return null;

            return _context.ShopInfo.FirstOrDefault(t => string.Compare(t.Title, title, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public IQueryable<ShopInfo> GetAll()
        {
            return _context.ShopInfo.AsQueryable();
        }
    }
}
