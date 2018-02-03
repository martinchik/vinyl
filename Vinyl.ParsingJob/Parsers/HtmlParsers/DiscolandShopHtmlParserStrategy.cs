using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public class DiscolandShopHtmlParserStrategy : BaseHtmlParserStrategy
    {
        private string[] _urlTemplates;

        public DiscolandShopHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null) 
            : base(logger, htmlDataGetter, dataLimit, true)
        {
        }

        public IParserStrategy Initialize(string urlTemplate = "https://discoland.by/index.php?categoryID=92&show_all=yes&inst=no;https://discoland.by/index.php?categoryID=91&show_all=yes&inst=no")
        {
            _urlTemplates = urlTemplate.Split(";").SelectMany(url => LoadAndGetRecordNodes(url, "//div[contains(@class, 'product-page_show-product-element')]//a", CancellationToken.None, true)
                .Select(_ => _.GetAttributeValue("href", string.Empty))
                .Where(_ => !string.IsNullOrEmpty(_) && _.Contains("product"))
                .Select(_ => "https://discoland.by/" + _)
                ).Distinct().ToArray();

            if (_urlTemplates?.Any() != true)
                throw new Exception("Products did not find on site " + urlTemplate);

            return this;
        }

        protected override string Name => "DiscolandShopHtml";

        protected override string GetNextPageUrl(int pageIndex)
            => pageIndex <= _urlTemplates.Length ? _urlTemplates[pageIndex - 1] : string.Empty;

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(int pageIndex, string pageData, CancellationToken token)
        {
            var rootNode = GetRecordNodes(pageData, "//div[contains(@class, 'main-content_info')]").FirstOrDefault();
            if (rootNode != null)
                return new[] { ParseRecordsFromDiscolandUrl(GetNextPageUrl(pageIndex), rootNode, token) };

            return Enumerable.Empty<DirtyRecord>();
        }

        private DirtyRecord ParseRecordsFromDiscolandUrl(string url, HtmlNode blogNode, CancellationToken token)
        {
            if (blogNode == null)
                return null;

            try
            {
                DirtyRecord record = new DirtyRecord();
                record.Url = url;

                var priceNode = blogNode.SelectSingleNode("//div[contains(@class, 'product-detail_price')]");
                if (priceNode != null)
                {
                    var allPriceText = priceNode.InnerText.WrapHtmlLines();
                    var allPriceLowwerText = allPriceText.ToLower();

                    record.Price = ExtractValueBy("цена:", allPriceLowwerText, allPriceText)
                        .Replace("руб", "бел").Replace("Руб", "бел");
                    if (!string.IsNullOrEmpty(record.Price) && record.Price.Last() == '.')
                        record.Price = record.Price.Substring(0, record.Price.Length - 1);
                    record.Barcode = ExtractValueBy("штрих-код:", allPriceLowwerText, allPriceText).Replace(".", "");
                    if (string.IsNullOrEmpty(record.Barcode))
                        record.Barcode = ExtractValueBy("штрихкод:", allPriceLowwerText, allPriceText).Replace(".", "");
                }

                var productNode = blogNode.SelectSingleNode("//div[contains(@class, 'product-detail_product-info')]");
                if (productNode != null)
                {
                    var allProductText = productNode.InnerText.WrapHtmlLines();
                    var allProductLowwerText = allProductText.ToLower();

                    record.Info = ExtractValueBy("характеристики:", allProductLowwerText, allProductText).Replace(".", "");
                    record.Style = ExtractValueBy("жанр:", allProductLowwerText, allProductText).Replace(".", "");
                    record.Year = ExtractValueBy("год первого выхода:", allProductLowwerText, allProductText).Replace(".", "");
                    if (string.IsNullOrEmpty(record.Year))
                        record.Year = ExtractValueBy("год выхода:", allProductLowwerText, allProductText).Replace(".", "");
                    record.YearRecorded = ExtractValueBy("переиздание:", allProductLowwerText, allProductText).Replace(".", "");
                    if (string.IsNullOrEmpty(record.YearRecorded))
                        record.YearRecorded = ExtractValueBy("издатель на виниле:", allProductLowwerText, allProductText).Replace(".", "");
                    record.Country = ExtractValueBy("произведено:", allProductLowwerText, allProductText).Replace(".", "");

                    if (!string.IsNullOrEmpty(record.Year))
                    {
                        var ind = record.Year.LastIndexOf('(');
                        if (ind > 0)
                            record.Year = record.Year.Substring(0, ind - 1).Trim();
                    }
                }

                var imageNode = blogNode.SelectSingleNode("//div[contains(@class, 'product-detail_pic')]");
                if (imageNode != null)
                {
                    var image = imageNode.Descendants("img").FirstOrDefault();
                    if (image != null)
                    {
                        record.ImageUrl = image.GetAttributeValue("src", "");
                        if (!string.IsNullOrEmpty(record.ImageUrl))
                            record.ImageUrl = "https://discoland.by/" + record.ImageUrl;

                        record.Title = image.GetAttributeValue("alt", "");
                    }
                }
                if (!string.IsNullOrEmpty(record.Title))
                {
                    var title = record.Title.Replace("!", ".").Replace("?", ".");
                    var items = title.Split(new[] { " - ", " – ", " — ", " — " }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length < 2)
                        items = title.Split(new[] { "-", "‎–", "–" }, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length == 2)
                    {
                        record.Artist = items[0];
                        var ind = items[1].LastIndexOf('(');
                        if (ind > 0)
                            record.Album = items[1].Substring(0, ind - 1);
                    }
                    else
                    {
                    }

                    record.State = "ЗАПЕЧАТАНА";

                    var optionParts = ParseSpecialFields.ExtractPartBetweenString(title, '(', ')', true).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => _.Length > 0).ToArray();
                    if (optionParts.Length > 0)
                    {
                        record.CountInPack = optionParts.First();
                        
                        if (optionParts.Length > 1)
                            record.View = optionParts.Last();
                    }
                }
                else
                {
                }
                                
                return record;
            }
            catch (Exception parseExc)
            {
                _logger.LogWarning(parseExc, $"Parsing record from page '{url}' has errors");
            }

            return null;
        }        
    }
}
