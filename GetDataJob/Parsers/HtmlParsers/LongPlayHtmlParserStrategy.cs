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
    public class LongPlayHtmlParserStrategy : IParserStrategy
    {
        private readonly IHtmlDataGetter _htmlDataGetter;
        private readonly IDirtyRecordProcessor _recordProcessor;
        private readonly ILogger _logger;
        private readonly int _pageSize = 96;
        private readonly int _degreeOfParalellism = 10;
        private readonly string _urlTemplate;

        public LongPlayHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor,
            string urlTemplate = "http://longplay.by/vse-stili.html?ditto_111_display=96&ditto_111_sortBy=pagetitle&ditto_111_sortDir=ASC&111_start={0}&111_start={1}"
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
                    var pageData = await _htmlDataGetter.GetPage(string.Format(_urlTemplate, _pageSize, pageIndex * _pageSize), token);

                    GetRecordNodes(pageData)
                        .AsParallel()
                        .WithCancellation(token)
                        .WithDegreeOfParallelism(_degreeOfParalellism)
                        .ForAll(recordNode =>
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var dirtyRecord = GetDataFromRecordNode(recordNode, token).GetAwaiter().GetResult();
                            _recordProcessor.AddRecord("LongPlayHtml", dirtyRecord);
                            readedPageCount++;
                        }
                        catch (Exception parseExc)
                        {
                            _logger.LogWarning(parseExc, "Parsing record from page has errors");
                        }
                    });

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

        private IEnumerable<HtmlNode> GetRecordNodes(string htmlPageData)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlPageData);
            foreach (var node in doc.DocumentNode.SelectNodes("//div[contains(@class, 'block_all')]//div[contains(@class, 'shs-descr')]"))
            {
                yield return node;
            }
        }

        private async Task<DirtyRecord> GetDataFromRecordNode(HtmlNode node, CancellationToken token)
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
                    record.Title = record.Album = ParseNodeValue(subNode.ChildNodes[3]);
                    record.Info = string.Concat("Style:", ParseNodeValue(subNode.ChildNodes[5]));
                    record.Price = ParseNodeValue(subNode.ChildNodes[7]);
                }
            }

            await ParseAdditionalData(record, token);
            return record;
        }

        private string ParseNodeValue(HtmlNode node)
        {
            var items = node.InnerText.Split(':');
            return (items.Length > 0 ? items[1] : node.InnerText).ToNormalValue();
        }

        private async Task ParseAdditionalData(DirtyRecord record, CancellationToken token)
        {
            if (string.IsNullOrEmpty(record?.Url))
                return;

            try
            {
                var subPage = await _htmlDataGetter.GetPage(record.Url, token);
                var tableMap = ParseTable(subPage).Where(_=>_.Item1.Length >0).ToDictionary(_=>_.Item1.ToLower(), _=>_.Item2);
                if (tableMap?.Count > 7)
                {
                    record.Artist = tableMap.GetSafeValue("исполнитель");
                    record.Album = tableMap.GetSafeValue("альбом");
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Concat("Country:", tableMap.GetSafeValue("страна")));
                    sb.AppendLine(string.Concat("Label:", tableMap.GetSafeValue("лейбл")));
                    sb.AppendLine(string.Concat("YearRecorded:", tableMap.GetSafeValue("год записи")));
                    sb.AppendLine(string.Concat("Style:", tableMap.GetSafeValue("стиль")));
                    record.Year = tableMap.GetSafeValue("год издания");
                    record.State = tableMap.GetSafeValue("состояние");
                    record.Info = sb.ToString();
                }
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Parsing personal record page has errors");
            }
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
