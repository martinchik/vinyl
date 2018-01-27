using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Vinyl.Common;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.Site.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMetadataRepositoriesFactory _db;
        private readonly ILogger _logger;

        public IndexModel(IMetadataRepositoriesFactory db, ILogger<PageModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public string SearchText { get; private set; } = string.Empty;

        public IList<SearchItem> Items { get; private set; } = null;

        public string GetSecondLineText(SearchItem item)
            => $"В {item?.ShopsCount} магазинах".AddIfExist(" в состоянии (", item?.States, ")");

        public string GetPriceText(SearchItem item)
        {
            if (item.PriceFrom == item.PriceTo)
                return item.PriceFrom?.ToString("F2");
            else if (!(item.PriceFrom > 0))
                return item.PriceTo?.ToString("F2");
            else if (!(item.PriceTo > 0))
                return item.PriceFrom?.ToString("F2");
            else
                return $"от {item.PriceFrom?.ToString("F2")} по {item.PriceTo?.ToString("F2")}";
        }

        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "search" })]
        public IActionResult OnGet(string search = null)
        {
            SearchText = search;

            try
            {
                search = ParseSpecialFields.DistinctWords(search);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    using (var rep = _db.CreateSearchItemRepository())
                    {
                        Items = rep.Find(search, "BY")
                            .OrderByDescending(_ => _.Sell)
                            .ThenBy(_ => _.PriceFrom)
                            .Take(100)
                            .ToList();
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "index OnGet exception");
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
                url = rep.FindPreview(recordId);
            }
            return RedirectPermanent(string.IsNullOrEmpty(url) ? "./img/noimage.png" : url);
        }
    }
}
