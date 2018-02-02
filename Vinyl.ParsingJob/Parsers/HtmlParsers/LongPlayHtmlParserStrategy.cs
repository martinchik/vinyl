using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public class LongPlayHtmlParserStrategy : BaseHtmlParserStrategy
    {
        private readonly int _pageSize = 96;
        private string _urlTemplate;

        public LongPlayHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null) 
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        public IParserStrategy Initialize(string urlTemplate)
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            return this;
        }

        protected override string Name => "LongPlayHtml";

        protected override string GetNextPageUrl(int pageIndex)
            => string.Format(_urlTemplate, _pageSize, pageIndex * _pageSize);
        
        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(int pageIndex, string pageData, CancellationToken token)
        {
            try
            {
                return GetRecordNodes(pageData, "//div[contains(@class, 'block_all')]//div[contains(@class, 'shs-descr')]").Select(recordNode =>
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        return GetDataFromRecordNode(recordNode, token).GetAwaiter().GetResult();
                    }
                    catch (Exception parseExc)
                    {
                        _logger.LogWarning(parseExc, "Parsing record from page has errors");
                    }
                    return null;
                });
            }
            catch (Exception parseExc)
            {
                _logger.LogWarning(parseExc, "Parsing record from page has errors");
            }
            return Enumerable.Empty<DirtyRecord>();
        }                

        private Task<DirtyRecord> GetDataFromRecordNode(HtmlNode node, CancellationToken token)
        {
            DirtyRecord record = new DirtyRecord();
            foreach (var subNode in node.ChildNodes)
            {
                if (subNode.OriginalName == "a" && !subNode.HasChildNodes)
                {
                    record.Url = subNode.GetAttributeValue("href", "");
                }
                else if (subNode.HasClass("bloopis"))
                {
                    record.Artist = ParseNodeValue(subNode.ChildNodes[1]);
                    record.Album = ParseNodeValue(subNode.ChildNodes[3]);
                    record.Style = ParseNodeValue(subNode.ChildNodes[5]);
                    record.Price = ParseNodeValue(subNode.ChildNodes[7]) + " бр";

                    record.Title = $"{record.Artist} - {record.Album}";
                }
            }

            return ParseAdditionalData(record, token);
        }

        private async Task<DirtyRecord> ParseAdditionalData(DirtyRecord record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(record?.Url))
                return record;

            try
            {
                var subPage = await _htmlDataGetter.GetPage(record.Url, token);

                var imageLink = ParseImage(subPage).FirstOrDefault();
                if (!string.IsNullOrEmpty(imageLink))
                    record.ImageUrl = imageLink;

                var tableMap = ParseTable(subPage).Where(_=>_.Item1.Length > 0).ToDictionary(_=>_.Item1.ToLower(), _=>_.Item2);
                if (tableMap?.Count > 7)
                {
                    record.Artist = tableMap.GetSafeValue("исполнитель");
                    record.Album = tableMap.GetSafeValue("альбом");
                    record.Year = tableMap.GetSafeValue("год издания");
                    record.State = tableMap.GetSafeValue("состояние");
                    record.Country = tableMap.GetSafeValue("страна");
                    record.Label = tableMap.GetSafeValue("лейбл");
                    record.YearRecorded = tableMap.GetSafeValue("год записи");

                    record.Info = tableMap.GetSafeValue("комментарий");
                    record.Price = tableMap.GetSafeValue("цена") + " бр";
                }
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Parsing personal record page has errors");
            }

            return record;
        }
        
        private IEnumerable<(string, string)> ParseTable(string html)
        {
            foreach (var tableLine in GetRecordNodes(html, "//table[contains(@class, 'zebra')]//tr"))
                yield return (ParseNodeTableValue(tableLine.ChildNodes[1]), ParseNodeTableValue(tableLine.ChildNodes[3]));
        }

        private IEnumerable<string> ParseImage(string html)
        {
            foreach (var imgItem in GetRecordNodes(html, "//article//a[contains(@class, 'fancybox')]//img"))
                yield return "http://longplay.by/" + imgItem.GetAttributeValue("src", "");
        }
    }
}
