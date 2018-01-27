using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.Site.Pages
{
    public class AboutModel : PageModel
    {
        private readonly IMetadataRepositoriesFactory _db;
        private readonly ILogger _logger;

        public AboutModel(IMetadataRepositoriesFactory db, ILogger<PageModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public StatisticsItem Statistic { get; private set; }

        [ResponseCache(Duration = 3600)]
        public async Task OnGet()
        {
            try
            {
                using (var rep = _db.CreateStatisticRepository())
                {
                    Statistic = await rep.GetStats();
                }
            }
            catch (Exception exc)
            {
                _logger.LogCritical(exc, "AboutPage OnGet exception");
                StatusCode((int)HttpStatusCode.InternalServerError, exc.Message);
            }
        }
    }
}
