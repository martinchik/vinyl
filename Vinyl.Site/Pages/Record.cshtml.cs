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

        public IEnumerable<(string text, string link)> GetLinksBy(Vinyl.Metadata.RecordLinkType linkType)
           => Record?.RecordLinks?.Where(_ => _.ToType == (int)linkType).Select(_ => (_.Text, _.Link));

        public Dictionary<Vinyl.Metadata.RecordLinkType, List<(string text, string link)>> GetLinks()
           => Record?.RecordLinks?
                .ToLookup(_ => (Vinyl.Metadata.RecordLinkType)_.ToType)
                .ToDictionary(_ => _.Key, _ => _.Select(v=>(v.Text, v.Link))
                .ToList());

        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "id" })]
        public IActionResult OnGet(string id = null)
        {
            Shops = null;
            Record = null;
           
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

        [ResponseCache(Duration = 360, VaryByQueryKeys = new[] { "id" })]
        public ActionResult OnGetRecordImage(string id = null)
        {
            Guid recordId = id.ExpandToGuid();
            if (recordId == Guid.Empty)
                return new NotFoundResult();

            string url = string.Empty;
            using (var rep = _db.CreateRecordArtRepository())
            {
                url = rep.FindFullImage(recordId);
            }
            return RedirectPermanent(string.IsNullOrEmpty(url) ? "./images/noimage.png" : url);
        }
    }
}