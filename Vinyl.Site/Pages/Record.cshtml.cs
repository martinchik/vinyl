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

        public string ToYouTubeLink()
            => $"https://www.youtube.com/results?search_query={Record?.Artist}".AddIfExist(" ", Record?.Album);

        public string ToYouTubeLink(string trackName)
           => $"https://www.youtube.com/results?search_query={Record?.Artist}".AddIfExist(" ", trackName);

        public bool IsActive(RecordInShopLink link)
            => link.Status == (int)Metadata.StrategyStatus.Active;

        public IEnumerable<(string text, string link)> GetLinksBy(Vinyl.Metadata.RecordLinkType linkType)
           => Record?.RecordLinks?.Where(_ => _.ToType == (int)linkType && !string.IsNullOrEmpty(_.Text) && !string.IsNullOrEmpty(_.Link)).Select(_ => (_.Text, _.Link));

        public (string text, string link) GetDiscogsLink()
           => Record?.RecordLinks?.Where(_ => _.ToType == (int)Vinyl.Metadata.RecordLinkType.Discogs)
                .Select(v => (v.Text, v.Link))
                .FirstOrDefault() ?? ("Не найдено", "#");

        public IEnumerable<string> GetDiscogsTracks()
           => Record?.RecordLinks?.FirstOrDefault(_ => _.ToType == (int)Vinyl.Metadata.RecordLinkType.Discogs)
               .Tracks?.Split("|") ?? new string[] { };

        public IEnumerable<(string title, string url)> GetDiscogsVideos()
          => Record?.RecordLinks?.FirstOrDefault(_ => _.ToType == (int)Vinyl.Metadata.RecordLinkType.Discogs)
               .Videos?.Split("|").Select(line => 
               {
                   var videoItem = line.Split("$");
                   return videoItem.Length > 1 ? (videoItem[0], videoItem[1]) : (string.Empty, string.Empty);
               }) ?? new (string, string)[] { };

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
            return RedirectPermanent(string.IsNullOrEmpty(url) ? "./img/noimage.png" : url);
        }
    }
}