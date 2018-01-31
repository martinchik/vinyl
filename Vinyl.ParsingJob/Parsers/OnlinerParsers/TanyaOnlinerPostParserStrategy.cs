using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.OnlinerParsers
{
    public class TanyaOnlinerPostParserStrategy : BaseOnlinerParserStrategy
    {
        public TanyaOnlinerPostParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }


        protected override string Name => "Ta-nya.Onliner";

        protected override DirtyRecord ParseRecordFromOnliner(string text, string imageUrl)
        {
            DirtyRecord record = new DirtyRecord();
            var index = text.LastIndexOf("(");
            var title = text;
            if (index > 0)
            {
                title = text.Substring(0, index);

                var indexLast = text.IndexOf(")", index);
                if (indexLast > 0)
                {
                    var innerText = text.Substring(index + 1, indexLast - index - 1);
                    var subItems = innerText.Split(",").Select(_ => _.Trim()).ToArray();

                    if (subItems.Length > 0)
                    {
                        record.Year = subItems.Where(_ => _.Length == 4)
                            .Select(_ => ParseSpecialFields.ParseYear(_)).Where(_ => _ != null)
                            .Select(_ => _.Value.ToString())
                            .FirstOrDefault() ?? string.Empty;

                        record.Info = string.Join(", ", subItems.Where(_ => _ != record.Year.ToString()));
                    }

                    if (text.Length > (indexLast + 3))
                    {
                        var lastText = text.Substring(indexLast+1, text.Length - indexLast - 1).Replace("/", string.Empty).Trim();
                        if (!string.IsNullOrEmpty(lastText))
                        {
                            subItems = lastText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray();
                            record.Price = subItems.Where(_ => _.Contains("$") && _.Length < 6).FirstOrDefault();
                            if (!string.IsNullOrEmpty(record.Price))
                                lastText = lastText.Replace(record.Price, string.Empty).Trim();
                        }
                        if (!string.IsNullOrEmpty(lastText))
                        {
                            if (string.IsNullOrEmpty(record.Info))
                                record.Info = lastText;
                            else
                                record.Info += Environment.NewLine + lastText;
                        }
                    }
                }
            }

            record.Title = title.Trim();
            record.ImageUrl = imageUrl;

            var items = title.Split(new[] { " - ", " – " }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length < 2)
                items = title.Split(new[] { "-", "‎–", "–" }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length >= 2)
            {
                record.Artist = items[0].Trim();
                if (items.Length == 2)
                    record.Album = items[1].Trim();
                else
                    record.Album = string.Join(" ", items.Skip(0));
            }

            if (string.IsNullOrEmpty(record.Artist) && string.IsNullOrEmpty(record.Album))
                return null;

            return record;
        }
    }
}
