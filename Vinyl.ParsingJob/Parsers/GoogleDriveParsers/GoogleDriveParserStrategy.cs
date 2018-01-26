using ExcelDataReader;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
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
        private const int KB = 0x400;
        private readonly int _downloadChunkSize = 256 * KB;

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

        private async Task<DriveService> CoonectToGoogle(CancellationToken token)
        {
            UserCredential credential;

            try
            {
                var assembly = typeof(GoogleDriveParserStrategy).Assembly;
                var resourceStream = assembly.GetManifestResourceStream("Vinyl.ParsingJob.Parsers.GoogleDriveParsers.client_secret.json");
                if (resourceStream == null)
                    throw new Exception("Google drive key did not find.");
                
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(resourceStream).Secrets, Scopes, "user", token);

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
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

            DriveService service = await CoonectToGoogle(token);

            var url = GetLastFileFromFolder(service, folderId);

            await DownloadFile(service, url, fileName, token);

            return fileName;
        }

        private string GetLastFileFromFolder(DriveService service, string folder)
        {
            var request = service.Files.List();
            request.Q = $"'{folder}' in parents";
            request.OrderBy = "createdDate desc";
            request.MaxResults = 2;

            var result = request.Execute();
            var file = result.Items.OrderByDescending(_ => _.CreatedDate).FirstOrDefault();

            return file.DownloadUrl;
        }

        private async Task DownloadFile(DriveService service, string url, string fileName, CancellationToken token)
        {
            var downloader = new MediaDownloader(service);
            downloader.ChunkSize = _downloadChunkSize;
           
            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var progress = await downloader.DownloadAsync(url, fileStream, token);
                if (progress.Status != DownloadStatus.Completed)
                    throw new Exception(string.Format("Download {0} was interpreted in the middle. Only {1} were downloaded. ", fileName, progress.BytesDownloaded));
            }
        }
    }
}
