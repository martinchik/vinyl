using ExcelDataReader;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
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
using Vinyl.Common;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Parsers.GoogleDriveParsers
{
    public abstract class GoogleDriveParserStrategy : BaseParserStrategy
    {
        private string _folderId;

        static string[] Scopes = { DriveService.Scope.DriveReadonly };

        public GoogleDriveParserStrategy(ILogger logger, IHtmlDataGetter htmlDataGetter, int? dataLimit = null)
            : base(logger, htmlDataGetter, dataLimit)
        {
        }

        public virtual IParserStrategy Initialize(string googleFolderId)
        {
            _folderId = googleFolderId ?? throw new ArgumentNullException(nameof(googleFolderId));
            return this;
        }

        protected override string GetNextPageUrl(int pageIndex)
            => pageIndex == 1 ? _folderId : string.Empty;

        protected override async Task<string> DownloadPageHtml(int pageIndex, CancellationToken token)
        {
            return await DownloadLastFileFromGoogleFolder(GetNextPageUrl(pageIndex), token);
        }

        protected override IEnumerable<DirtyRecord> ParseRecordsFromPage(string pageData, CancellationToken token)
        {
            var fileName = pageData;
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    return ParseFileFromGoogleDrive(fileName);
                }
            }
            finally
            {
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
            }

            return Enumerable.Empty<DirtyRecord>();
        }

        protected abstract IEnumerable<DirtyRecord> ParseFileFromGoogleDrive(string fileName);        

        private DriveService CoonectToGoogle(CancellationToken token)
        {
            try
            {
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    ApiKey = GlobalConstants.GoogleApiKey,                    
                    ApplicationName = GlobalConstants.ApplicationName,
                });
                
                return service;
            }
            catch (Exception exc)
            {
                System.Diagnostics.Trace.WriteLine($"Error during getting Google Drive service. Exception:" + exc.ToString());
                throw;
            }
        }

        private async Task<string> DownloadLastFileFromGoogleFolder(string folderId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(folderId))
                return string.Empty;

            var fileName = Path.ChangeExtension(Path.GetTempFileName(), "xls");

            DriveService service = CoonectToGoogle(token);

            var fileId = await GetLastFileFromFolder(service, folderId, token);

            await DownloadFile(service, fileId, fileName, token);

            return fileName;
        }

        private async Task<string> GetLastFileFromFolder(DriveService service, string folder, CancellationToken token)
        {
            var request = service.Files.List();
            request.Q = $"'{folder}' in parents";
            request.OrderBy = "createdTime desc";
            request.Spaces = "drive";
            request.PageSize = 2;

            var result = await request.ExecuteAsync(token);
            var file = result.Files.OrderByDescending(_ => _.CreatedTime).FirstOrDefault();

            return file.Id;
        }

        private async Task DownloadFile(DriveService service, string fileId, string fileName, CancellationToken token)
        {
            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var file = service.Files.Get(fileId);
                var progress = await file.DownloadAsync(fileStream, token);
                if (progress.Status != DownloadStatus.Completed)
                    throw new Exception(string.Format("Download {0} was interpreted in the middle. Only {1} were downloaded. ", fileName, progress.BytesDownloaded));
            }
        }
    }
}
