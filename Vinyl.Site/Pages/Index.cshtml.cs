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
    public class IndexModel : PageModel
    {
        private readonly IMetadataRepositoriesFactory _db;

        public IndexModel(IMetadataRepositoriesFactory db)
        {
            _db = db;
        }

        public string SearchText { get; private set; } = string.Empty;

        public IList<SearchItem> Items { get; private set; } = null;
        
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "search" })]
        public IActionResult OnGet(string search = null)
        {
            SearchText = search;

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

            return Page();
        }
    }
}
