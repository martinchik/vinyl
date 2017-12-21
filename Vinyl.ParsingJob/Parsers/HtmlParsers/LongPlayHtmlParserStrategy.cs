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
    public class LongPlayHtmlParserStrategy : BaseParserStrategy
    {
        private readonly int _pageSize = 96;
        private readonly int _degreeOfParalellism = 10;
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
        {
            return string.Format(_urlTemplate, _pageSize, pageIndex * _pageSize);
        }

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(string pageData, CancellationToken token) =>  
            GetRecordNodes(pageData)
                .AsParallel()
                .WithCancellation(token)
                .WithDegreeOfParallelism(_degreeOfParalellism)
                .Select(recordNode =>
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
        

        private IEnumerable<HtmlNode> GetRecordNodes(string htmlPageData)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlPageData);
            foreach (var node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'block_all')]//div[contains(@class, 'shs-descr')]"))
            {
                yield return node;
            }
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
                    record.Price = ParseNodeValue(subNode.ChildNodes[7]) + " белр";

                    record.Title = $"{record.Artist} - {record.Album}";
                }
            }

            return ParseAdditionalData(record, token);
        }

        private string ParseNodeValue(HtmlNode node)
        {
            var items = node.InnerText.Split(':');
            return (items.Length > 0 ? items[1] : node.InnerText).ToNormalValue();
        }

        private async Task<DirtyRecord> ParseAdditionalData(DirtyRecord record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(record?.Url))
                return record;

            try
            {
                var subPage = await _htmlDataGetter.GetPage(record.Url, token);
                var tableMap = ParseTable(subPage).Where(_=>_.Item1.Length >0).ToDictionary(_=>_.Item1.ToLower(), _=>_.Item2);
                if (tableMap?.Count > 7)
                {
                    record.Artist = tableMap.GetSafeValue("исполнитель");
                    record.Album = tableMap.GetSafeValue("альбом");
                    record.Year = tableMap.GetSafeValue("год издания");
                    record.State = tableMap.GetSafeValue("состояние");
                    record.Country = tableMap.GetSafeValue("страна");
                    record.Label = tableMap.GetSafeValue("лейбл");
                    record.YearRecorded = tableMap.GetSafeValue("год записи");
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
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            foreach (var tableLine in doc.DocumentNode.SelectNodes("//table[contains(@class, 'zebra')]//tr"))
            {                
                yield return (ParseNodeTableValue(tableLine.ChildNodes[1]), ParseNodeTableValue(tableLine.ChildNodes[3]));
            }
        }

        private string ParseNodeTableValue(HtmlNode node)
        {
            return node.InnerText.ToNormalValue()
                .Replace(":", string.Empty);
        }
    }
}
