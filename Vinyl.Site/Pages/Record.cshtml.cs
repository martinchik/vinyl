using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.Site.Pages
{
    public class RecordModel : PageModel
    {
        private readonly IMetadataRepositoriesFactory _db;

        public RecordModel(IMetadataRepositoriesFactory db)
        {
            _db = db;
        }

        public RecordInfo Record { get; private set; }

        public IList<ShopInfo> Shops { get; private set; }        

        public ShopInfo GetShopBy(RecordInShopLink link)
            => link?.ShopId != Guid.Empty && Shops?.Any() == true
                ? Shops.FirstOrDefault(_ => _.Id == link.ShopId)
                : null;

        public bool IsActive(RecordInShopLink link)
            => link.Status == (int)Metadata.StrategyStatus.Active;

        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "id" })]
        public IActionResult OnGet(string id = null)
        {
            Shops = null;
            Record = null;

            if (string.IsNullOrWhiteSpace(id))
                return new NotFoundResult();

            Guid recordId = id.ExpandToGuid();

            if (recordId == Guid.Empty)
                return new NotFoundResult();

            using (var rep = _db.CreateRecordInfoRepository())
            {
                Record = rep.GetFull(recordId);
            }

            if (Record.RecordInShopLink?.Any() == true)
            {
                var shopIds = Record.RecordInShopLink.Select(_ => _.ShopId).ToArray();
                if (shopIds.Any())
                {
                    using (var rep = _db.CreateShopInfoRepository())
                    {
                        Shops = rep.Get(shopIds).ToList();
                    }
                }
            }

            return Page();
        }
    }
}