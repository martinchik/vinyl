using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vinyl.DbLayer;
using Vinyl.Site.SiteMapModel;

namespace Vinyl.Site.Controllers
{
    public class SitemapController : Controller
    {
        private readonly ISitemapBuilder _builder;

        public SitemapController(ISitemapBuilder builder)
        {
            _builder = builder;
        }

        [Route("sitemap")]
        public ActionResult Sitemap()
        {            
            return Content(_builder.Build(), "text/xml");
        }
    }
}