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

        public IList<SearchItem> Items { get; private set; }

        public IActionResult OnPost(string searchText)
        {
            using (var rep = _db.CreateSearchItemRepository())
            {
                Items = rep.Find(searchText)
                    .OrderByDescending(_ => _.Sell)
                    .ThenBy(_ => _.PriceFrom)
                    .Take(100)
                    .ToList();
            }

            return Page();
        }
    }
}
