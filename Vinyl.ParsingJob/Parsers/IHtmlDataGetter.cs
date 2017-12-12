﻿using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.ParsingJob.Parsers
{
    public interface IHtmlDataGetter
    {
        Task<string> GetPage(string url, CancellationToken token = default(CancellationToken));
        Task<string> DownloadFileToLocal(string fileUrl, CancellationToken token = default(CancellationToken));
    }
}