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
    public class VinplazaShopHtmlParserStrategy : BaseHtmlParserStrategy
    {
        private string _urlTemplate;

        public VinplazaShopHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null) 
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        public IParserStrategy Initialize(string urlTemplate = "http://www.vinplaza.ru/sitemap.xml?page={0}")
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            return this;
        }

        protected override string Name => "VinplazaShopHtml";

        protected override string GetNextPageUrl(int pageIndex)
            => string.Format(_urlTemplate, pageIndex);

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(int pageIndex, string pageData, CancellationToken token)
        {            
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pageData);

            var urls = doc.GetElementsByTagName("loc").OfType<XmlNode>().Select(_ => _.InnerText).ToArray();

            return urls.Select(_ =>            
                ParseRecordsFromVinplazaUrl(_, LoadAndGetRecordNodes(_, "//div[contains(@itemprop, 'blogPost')]", token).FirstOrDefault(), token));
        }

        private DirtyRecord ParseRecordsFromVinplazaUrl(string url, HtmlNode blogNode, CancellationToken token)
        {
            if (blogNode == null)
                return null;
            
            try
            {
                DirtyRecord record = new DirtyRecord();
                record.Url = url;

                var titleNode = blogNode.Descendants("h3").FirstOrDefault();
                if (titleNode == null)
                    return null;

                var titleParseResult = ParseTitle(titleNode.InnerText.ToNormalValue());

                record.Title = titleParseResult.title;
                record.Artist = titleParseResult.artist;
                record.Album = titleParseResult.album;
                record.State = titleParseResult.state;
                record.Price = titleParseResult.price;
                record.Year = titleParseResult.year;

                var images = blogNode.Descendants("img").Select(_ => (img: _, width: _.GetAttributeValue("width", 0))).ToArray();
                var imgNode = images.Where(_ => _.width > 100 && _.width < 500).Select(_ => _.img).FirstOrDefault();

                if (imgNode == null && images.Length > 0)
                    imgNode = images.Select(_=>_.img).FirstOrDefault();

                if (imgNode != null)
                {
                    record.ImageUrl = imgNode.GetAttributeValue("src", "");
                }

                var allText = blogNode.InnerHtml.WrapHtmlLines();
                var allLowwerText = allText.ToLower();

                record.Label = ExtractValueBy("label:", allLowwerText, allText);
                record.View = ExtractValueBy("format:", allLowwerText, allText);
                record.Country = ExtractValueBy("country:", allLowwerText, allText);
                record.YearRecorded = ExtractValueBy("released:", allLowwerText, allText);
                record.Style = ExtractValueBy("style:", allLowwerText, allText);
                
                return record;
            }
            catch (Exception parseExc)
            {
                _logger.LogWarning(parseExc, $"Parsing record from page '{url}' has errors");
            }

            return null;
        }


        private (string title, string album, string artist, string year, string price, string state) ParseTitle(string line)
        {
            string title = string.Empty;
            string album = string.Empty;
            string artist = string.Empty;
            string year = string.Empty;
            string price = string.Empty;
            string state = string.Empty;

            //1674. Manfred Mann. Angel Station. 1979. Bronze (DE, OiS w Lyrics, Poster, VG+) = 30$
            //1598. Black Sabbath. Technical Ecstasy. 1976. Vertigo (UK, OiS, VG+) = 53$
            //8. Genesis. Abacab. 1981. Vertigo (DE, OiS, NM-) = 18$
            //1.Genesis.Foxtrot. 1972.Charisma(DE, FOC) = 18$
            //1695. Jean Michel Jarre. Zoolook. 1984. Polydor (DE, OiS, NM-)

            var parts = line.Replace("!", ".").Replace("?", ".").Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_=>_.Length>0).ToArray();
            var numbersParts = parts.Select((val, index) =>
            {
                var num = ParseSpecialFields.ParseNumber(val);
                return (number: num > 0 ? num.Value : 0, index: index);
            }).Where(_ => _.number > 0).ToArray();

            if (numbersParts.Length >= 2)
            {
                var firstNumber = numbersParts[0].index;
                var secondNumber = numbersParts[1].index;
                var titleParts = parts.Skip(firstNumber + 1).Take(secondNumber - firstNumber - 1).ToArray();

                title = string.Join(" - ", titleParts);
                year = numbersParts.Last().number.ToString();

                if (titleParts.Length != 2)
                    titleParts = title.Split(" - ");

                if (titleParts.Length == 1)
                    artist = album = title;
                else if (titleParts.Length == 2)
                {
                    artist = titleParts[0];
                    album = titleParts[1];
                }
                else if (titleParts.Length > 0)
                {
                    artist = titleParts[0];
                    album = string.Join(" - ", titleParts.Skip(1));
                }
            }
            else
            {
                if (parts.Length > 2)
                {
                    var titleParts = parts.Skip(1).Take(2).ToArray();

                    title = string.Join(" - ", titleParts);

                    if (titleParts.Length != 2)
                        titleParts = title.Split(" - ");

                    if (titleParts.Length == 2)
                    {
                        artist = titleParts[0];
                        album = titleParts[1];
                    }
                }
            }

            var lastPart = parts.Last();
            var priceParts = lastPart.Split(new[] { '=' }, StringSplitOptions.None).Select(_ => _.Trim()).Where(_ => _.Length > 0).ToArray();
            if (priceParts.Length > 1)
                price = priceParts.Last();

            var optionParts = ParseSpecialFields.ExtractPartBetweenString(lastPart, '(', ')', true).Split(new[] { ','}, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => _.Length > 0).ToArray();
            if (optionParts.Length > 1)
                state = optionParts.Last();

            if (string.IsNullOrEmpty(title))
                title = line;

            return (title, album, artist, year, price, state);
        }
    }
}
