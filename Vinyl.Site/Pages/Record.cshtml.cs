using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.Site.Pages
{
    public class RecordModel : PageModel
    {
        private readonly IMetadataRepositoriesFactory _db;
        private readonly ILogger _logger;

        public RecordModel(IMetadataRepositoriesFactory db, ILogger<PageModel> logger)
        {
            _db = db;
            _logger = logger;
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
               .Videos?.Split("|").Where(_ => !string.IsNullOrEmpty(_)).Select(line => 
               {
                   var videoItem = line.Split("$");
                   return videoItem.Length > 1 ? (videoItem[0], videoItem[1]) : (string.Empty, string.Empty);
               }) ?? new (string, string)[] { };

        [ResponseCache(Duration = 360, VaryByQueryKeys = new[] { "name" })]
        public IActionResult OnGet(string name)
        {
            Shops = null;
            Record = null;

            try
            {
                using (var rep = _db.CreateRecordInfoRepository())
                {
                    if (!string.IsNullOrEmpty(name))
                        Record = rep.GetFull(name);
                }

                if (Record == null)
                    return new NotFoundResult();

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
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "record OnGet exception");
                StatusCode((int)HttpStatusCode.InternalServerError, exc.Message);
            }
            return Page();
        }

        [ResponseCache(Duration = 360, VaryByQueryKeys = new[] { "id" })]
        public ActionResult OnGetRecordImage(Guid id)
        {
            Guid recordId = id;
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