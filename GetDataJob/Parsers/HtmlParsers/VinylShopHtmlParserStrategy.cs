using GetDataJob.Model;
using GetDataJob.Processor;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Parsers.HtmlParsers
{
    public class VinylShopHtmlParserStrategy : IParserStrategy
    {
        private readonly IHtmlDataGetter _htmlDataGetter;
        private readonly IDirtyRecordProcessor _recordProcessor;
        private readonly ILogger _logger;
        private readonly string _urlTemplate;

        public VinylShopHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor,
            string urlTemplate = "http://www.vinylshop.by/products/page/{0}/"
            )
        {
            _urlTemplate = urlTemplate;
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
            _recordProcessor = recordProcessor ?? throw new ArgumentNullException(nameof(recordProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(CancellationToken token)
        {
            try
            {
                int readedAllCount = 0;
                int readedPageCount = 0;
                int pageIndex = 1;

                do
                {
                    readedPageCount = 0;
                    var pageData = await _htmlDataGetter.GetPage(string.Format(_urlTemplate, pageIndex), token);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(pageData);

                    List<DirtyRecord> records = new List<DirtyRecord>();
                    GetRecordNodes(doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'archive-listing list')]"), records);

                    foreach (var record in records)
                    {                        
                        _recordProcessor.AddRecord("VinylShopHtml", record);
                        readedPageCount++;
                    }

                    pageIndex++;
                    readedAllCount += readedPageCount;
                }
                while (readedPageCount > 0);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "LongPlay html page error");
            }
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
                    var subNames = titleNode.InnerText.Split(" / ");
                    if (subNames.Length > 2)
                    {
                        record.Artist = subNames[0].ToNormalValue();
                        record.Album = record.Title = subNames[1].ToNormalValue();
                        record.Year = subNames[2].ToNormalValue();

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
                        record.Price = priceNode.InnerText.ToNormalValue().Replace("BYR", string.Empty).Trim();
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
