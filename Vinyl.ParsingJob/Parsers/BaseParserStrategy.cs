﻿using Vinyl.Metadata;
using Vinyl.ParsingJob.Processor;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public abstract class BaseParserStrategy : IParserStrategy
    {
        protected readonly IHtmlDataGetter _htmlDataGetter;
        protected readonly ILogger _logger;
        protected readonly int? _dataLimit;

        public BaseParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
        {
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _dataLimit = dataLimit;
        }

        protected virtual bool IsFileType { get => false; }

        protected abstract string Name { get; }

        protected abstract string GetNextPageUrl(int pageIndex);

        protected abstract IEnumerable<DirtyRecord> ParseRecordsFromPage(string pageData, CancellationToken token);        

        public IEnumerable<DirtyRecord> Parse(CancellationToken token)
        {
            int readedAllCount = 0;
            int readedPageCount = 0;
            int pageIndex = 1;
            Stopwatch sw = Stopwatch.StartNew();

            try
            {                
                do
                {
                    readedPageCount = 0;
                    var pageData = _htmlDataGetter.GetPage(GetNextPageUrl(pageIndex), token).GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(pageData))
                    {
                        foreach (var record in ParseRecordsFromPage(pageData, token))
                        {
                            if (_dataLimit <= (readedAllCount + readedPageCount))
                            {
                                readedPageCount = 0;
                                break;
                            }

                            yield return record;
                            readedPageCount++;
                        }
                    }

                    pageIndex++;
                    readedAllCount += readedPageCount;
                }
                while (readedPageCount > 0);                
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation(Name + $": Readed {readedAllCount} records from {pageIndex} pages. ElapsedTime: {sw.Elapsed}");
            }
        }        
    }
}