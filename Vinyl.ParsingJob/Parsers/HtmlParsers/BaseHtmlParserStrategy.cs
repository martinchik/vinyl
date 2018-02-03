using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vinyl.Common;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public abstract class BaseHtmlParserStrategy : BaseParserStrategy
    {
        public BaseHtmlParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null, bool useEncoding = false)
            : base(logger, htmlDataGetter, dataLimit, useEncoding)
        {
        }

        protected IEnumerable<HtmlNode> GetRecordNodes(string html, string searchPattern)
        {
            if (!string.IsNullOrWhiteSpace(html))
            {
                var rootNode = LoadDocumentFromHtml(html);
                if (rootNode?.HasChildNodes == true)
                {
                    foreach (var node in rootNode.SelectNodes(searchPattern))
                    {
                        yield return node;
                    }
                }
            }
        }

        protected IEnumerable<HtmlNode> LoadAndGetRecordNodes(string url, string searchPattern, CancellationToken token, bool useEncoding = false)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string html = string.Empty;
                try
                {
                    html = _htmlDataGetter.GetPage(url, token, useEncoding).GetAwaiter().GetResult();
                }
                catch (Exception exc)
                {
                    _logger.LogWarning(exc, "Error loading html by url " + url);
                }

                if (!string.IsNullOrWhiteSpace(html))
                {
                    var rootNode = LoadDocumentFromHtml(html);
                    if (rootNode?.HasChildNodes == true)
                    {
                        foreach (var node in rootNode.SelectNodes(searchPattern))
                        {
                            yield return node;
                        }
                    }
                }
            }
        }

        protected string ParseNodeValue(HtmlNode node)
        {
            var items = node.InnerText.Split(':');
            return (items.Length > 0 ? items[1] : node.InnerText).ToNormalValue();
        }

        protected string ParseNodeTableValue(HtmlNode node)
            => node.InnerText.ToNormalValue().Replace(":", string.Empty);

        protected HtmlNode LoadDocumentFromHtml(string html)
        {
            var doc = new HtmlDocument();
            try
            {
                doc.LoadHtml(html);
                return doc.DocumentNode;
            }
            catch (Exception exc)
            {
                var reasons = TryGetErrors(doc);
                if (string.IsNullOrEmpty(reasons))
                    _logger.LogWarning(exc, "Error loading html");
                else
                    _logger.LogWarning("Error loading html. Reasons:" + reasons);
            }

            return null;
        }

        protected string TryGetErrors(HtmlDocument doc)
        {
            try
            {
                if (doc?.ParseErrors?.Any() == true)
                {
                    return string.Join(";", doc.ParseErrors.Select(_ =>
                        $"{_.Reason} in code:{_.Code} and Line:{_.Line}"
                    ));
                }
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Error in getting errors from parse html results");
            }
            return string.Empty;
        }

        protected string ExtractValueBy(string name, string allLowwerText, string allText, bool toNewLine = true)
        {
            var valueIndex = allLowwerText.IndexOf(name);
            if (valueIndex >= 0)
            {
                var valueText = allText.Substring(valueIndex + name.Length, allText.Length - valueIndex - name.Length);
                if (!string.IsNullOrEmpty(valueText) && valueText.Length > 3)
                {
                    if (toNewLine)
                    {
                        var valueIndexTo = valueText.IndexOf("\n");
                        if (valueIndexTo < 0)
                            valueIndexTo = valueText.IndexOf(Environment.NewLine);
                        if (valueIndexTo > 0)
                        {
                            valueText = valueText.Substring(0, valueIndexTo);
                            if (string.IsNullOrEmpty(valueText) || valueText.Length < 3)
                                return string.Empty;
                        }
                    }
                    return valueText.ToNormalValue();
                }
            }

            return string.Empty;
        }
    }
}
