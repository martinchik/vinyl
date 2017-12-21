using Vinyl.Metadata;
using Vinyl.ParsingJob.Processor;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public class VinylShopHtmlParserStrategy : BaseParserStrategy
    {
        private string _urlTemplate;

        public VinylShopHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null) 
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        public IParserStrategy Initialize(string urlTemplate)
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            return this;
        }

        protected override string Name => "VinylShopHtml";

        protected override string GetNextPageUrl(int pageIndex)
        {
            return string.Format(_urlTemplate, pageIndex);
        }

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(string pageData, CancellationToken token)
        {            
            var doc = new HtmlDocument();
            doc.LoadHtml(pageData);

            List<DirtyRecord> records = new List<DirtyRecord>();
            GetRecordNodes(doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'archive-listing list')]"), records);
            return records;
        }

        private void GetRecordNodes(HtmlNode node, List<DirtyRecord> records)
        {
            try
            {                
                if (node == null)
                    return;

                var linkNode = node.Descendants("a").FirstOrDefault();
                if (linkNode == null)
                    return;

                DirtyRecord record = new DirtyRecord();
                record.Url = linkNode.GetAttributeValue("href", "");

                var titleNode = linkNode.Descendants("h3").FirstOrDefault();
                if (titleNode != null)
                {
                    var titleText = titleNode.InnerText;
                    var subNames = titleText.Split(" / ");
                    if (subNames.Length > 2)
                    {
                        record.Artist = subNames[0].ToNormalValue();
                        record.Album = subNames[1].ToNormalValue();
                        record.Year = subNames[2].ToNormalValue();

                        record.Title = titleText;

                        int v = record.Year.IndexOf("(");

                        if (v > 0)
                        {
                            record.Year = record.Year.Substring(0, v).Trim();                        
                        }
                    }
                }

                var descriptNode = linkNode.NextSibling;
                if (descriptNode != null)
                {
                    record.Info = descriptNode.Descendants("p").FirstOrDefault()?.InnerText.ToNormalValue() ?? string.Empty;
                    var priceNode = descriptNode.Descendants("span").FirstOrDefault()?.Descendants("span").FirstOrDefault();
                    if (priceNode != null)
                        record.Price = priceNode.InnerText.ToNormalValue();
                    //record.Price = priceNode.InnerText.ToNormalValue().Replace("BYR", string.Empty).Trim();
                }

                records.Add(record);
                GetRecordNodes(node.LastChild, records);
            }
            catch (Exception parseExc)
            {
                _logger.LogWarning(parseExc, "Parsing record from page has errors");
            }
        }
    }
}
