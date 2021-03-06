﻿using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public class VinylShopHtmlParserStrategy : BaseHtmlParserStrategy
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
            =>string.Format(_urlTemplate, pageIndex);        

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(int pageIndex, string pageData, CancellationToken token)
        {
            List<DirtyRecord> records = new List<DirtyRecord>();
            GetRecordNodes(LoadDocumentFromHtml(pageData).SelectSingleNode("//div[contains(@class, 'archive-listing list')]"), records);
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

                var imgNode = linkNode.Descendants("img").FirstOrDefault();
                if (imgNode != null)
                {
                    record.ImageUrl = imgNode.GetAttributeValue("src", "");
                }

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
                        int indexS = record.Year.Trim().ToLower().LastIndexOf("s");
                        if (v > 0)
                        {
                            record.Year = record.Year.Substring(0, v).Trim();
                        }
                        else if (indexS > 0)
                        {
                            int sYear;
                            if (int.TryParse(record.Year.Substring(0, indexS), out sYear))
                                record.Year = (1900 + sYear).ToString();
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
