using ExcelDataReader;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.OnlinerParsers
{
    public abstract class BaseOnlinerParserStrategy : BaseParserStrategy
    {
        private string[] _urlTemplates;
        private string _htmlTagClassName;

        public BaseOnlinerParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        public virtual IParserStrategy Initialize(string topicIds, string className = "b-msgpost-txt")
        {
            _urlTemplates = (topicIds ?? throw new ArgumentNullException(nameof(topicIds)))
                .Split(";").Where(_ => !string.IsNullOrEmpty(_) && _.Length > 3)
                .Select(_ => "https://baraholka.onliner.by/viewtopic.php?t="+ _).ToArray();

            _htmlTagClassName = className ?? throw new ArgumentNullException(nameof(className));
            if (_urlTemplates.Length == 0)
                throw new ArgumentNullException(nameof(topicIds));

            return this;
        }

        protected override string GetNextPageUrl(int pageIndex)
            => pageIndex <= _urlTemplates.Length ? _urlTemplates[pageIndex-1] : string.Empty;        

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(int pageIndex, string pageData, CancellationToken token)
        {
            var postNode = GetRootPostNode(pageData);
            if (postNode == null)
                return Enumerable.Empty<DirtyRecord>();

            try
            {
                return GetDataFromRecordNode(postNode, token).Select(_ => 
                {
                    _.Url = _urlTemplates[pageIndex - 1];
                    return _;
                });
            }
            catch (Exception parseExc)
            {
                _logger.LogWarning(parseExc, "Parsing record from page has errors");
            }

            return Enumerable.Empty<DirtyRecord>();
        }

        protected virtual IEnumerable<DirtyRecord> GetDataFromRecordNode(HtmlNode postNode, CancellationToken token)
        {
            string lastText = "";
            string imageSrc = "";
            foreach (var subNode in postNode.ChildNodes)
            {
                if (subNode.OriginalName == "p")
                {
                    if (subNode.HasClass("msgpost-img__p"))
                    {
                        var imgNode = subNode.Descendants("img").FirstOrDefault();
                        if (imgNode != null)
                        {
                            imageSrc = imgNode.GetAttributeValue("src", "");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(lastText))
                        {
                            var rec = ParseRecordFromOnliner(lastText, imageSrc);
                            if (rec != null)
                                yield return rec;
                        }

                        lastText = "";
                        imageSrc = "";

                        string dirtyTxt = subNode.InnerText;
                        if (!string.IsNullOrEmpty(dirtyTxt) && dirtyTxt.Length > 10)
                        {
                            var prevLine = "";
                            foreach (var line in dirtyTxt.Split(new[] { Environment.NewLine, "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (line.Length > 10 && line.Contains("("))
                                {
                                    if (!string.IsNullOrEmpty(prevLine))
                                    {
                                        var rec = ParseRecordFromOnliner(prevLine, string.Empty);
                                        if (rec != null)
                                            yield return rec;
                                    }
                                    prevLine = line.ToNormalValue();
                                }
                            }
                            lastText = prevLine;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(lastText))
            {
                var rec = ParseRecordFromOnliner(lastText, imageSrc);
                if (rec != null)
                    yield return rec;
            }
        }

        protected abstract DirtyRecord ParseRecordFromOnliner(string text, string imageUrl);

        private HtmlNode GetRootPostNode(string pageData)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(pageData);
            
            var bodyNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, '" + _htmlTagClassName + "')]//div[contains(@class, 'content')]");            

            return bodyNode;
        }
    }
}
