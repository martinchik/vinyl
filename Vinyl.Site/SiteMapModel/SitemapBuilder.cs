using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vinyl.DbLayer;

namespace Vinyl.Site.SiteMapModel
{
    public class SitemapBuilder : ISitemapBuilder
    {
        private readonly XNamespace NS = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly string baseUrl = "http://buy-vinyl.by/";

        private readonly IMetadataRepositoriesFactory _db;
        private readonly ILogger _logger;

        public SitemapBuilder(IMetadataRepositoriesFactory db, ILogger<SitemapBuilder> logger)
        {
            _db = db;
            _logger = logger;
        }

        public string Build()
            => new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(NS + "urlset", GetElements())
                ).ToString();        

        private IEnumerable<XElement> GetElements()
        {
            yield return CreateItemElement(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

            using (var rs = _db.CreateRecordInfoRepository())
                foreach (var record in rs.GetAllUrls())
                    yield return CreateItemElement(baseUrl + record, modified: (DateTime)DateTime.UtcNow, changeFrequency: null, priority: 0.9);
        }        

        private XElement CreateItemElement(string url, DateTime? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null)
        {
            XElement itemElement = new XElement(NS + "url", new XElement(NS + "loc", url.ToLower()));

            if (modified.HasValue)
            {
                itemElement.Add(new XElement(NS + "lastmod", modified.Value.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));
            }

            if (changeFrequency.HasValue)
            {
                itemElement.Add(new XElement(NS + "changefreq", changeFrequency.Value.ToString().ToLower()));
            }

            if (priority.HasValue)
            {
                itemElement.Add(new XElement(NS + "priority", priority.Value.ToString("N1")));
            }

            return itemElement;
        }
    }
}
