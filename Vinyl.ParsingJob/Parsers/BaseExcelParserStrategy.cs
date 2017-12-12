using ExcelDataReader;
using Vinyl.Metadata;
using Vinyl.ParsingJob.Processor;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.ParsingJob.Parsers.HtmlParsers
{
    public abstract class BaseExcelParserStrategy : BaseParserStrategy
    {
        private string _urlTemplate;
        private string _htmlTagClassName;
        private string _htmlTagLinkPartOfText;

        public BaseExcelParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, IDirtyRecordProcessor recordProcessor, int? dataLimit = null)
            : base(logger, htmlDataGetter, recordProcessor, dataLimit)
        {
        }

        public virtual IParserStrategy Initialize(string urlTemplate, string className = "post-content", string refLinkText = "СКАЧАТЬ")
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            _htmlTagClassName = className ?? throw new ArgumentNullException(nameof(className));
            _htmlTagLinkPartOfText = refLinkText ?? throw new ArgumentNullException(nameof(refLinkText));
            return this;
        }
        protected override string GetNextPageUrl(int pageIndex)
            => pageIndex == 1 ? _urlTemplate : string.Empty;        

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(string pageData, CancellationToken token)
        {
            var fileUrl = GetFileUrl(pageData);
            if (string.IsNullOrEmpty(fileUrl))
                return Enumerable.Empty<DirtyRecord>();

            var fileName = "";
            try
            {
                fileName = _htmlDataGetter.DownloadFileToLocal(fileUrl, token).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(fileName))
                {
                    return ParseDataFromDataSet(ReadExcelFile(fileName));
                }
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }

            return Enumerable.Empty<DirtyRecord>();
        }

        protected abstract IEnumerable<DirtyRecord> ParseDataFromDataSet(DataSet dataSet);

        private DataSet ReadExcelFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return reader.AsDataSet();                    
                }
            }
        }

        private string GetFileUrl(string pageData)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(pageData);
            
            var bodyNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, '" + _htmlTagClassName + "')]");
            if (bodyNode != null)
            {
                var linkNode = bodyNode.Descendants("a").Where(_ => _.InnerText.Contains(_htmlTagLinkPartOfText)).FirstOrDefault();
                if (linkNode != null)
                {
                    return linkNode.GetAttributeValue("href", "");
                }
            }

            return null;
        }
    }
}
