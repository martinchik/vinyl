using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GetDataJob.Parsers
{
    public class HtmlDataGetter : IHtmlDataGetter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public HtmlDataGetter(ILogger logger, HttpClient httpClient = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (_httpClient == null)
            {
                var clientHandler = new HttpClientHandler();
                clientHandler.Proxy = null;

                _httpClient = new HttpClient(clientHandler);
            }

            _httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        }

        public async Task<string> GetPage(string url, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            HttpResponseMessage response = null;

            try
            {
                _logger.LogInformation($"(ThreadId:{Thread.CurrentThread.ManagedThreadId}). Getting page from url:{url}");
                
                response = await _httpClient.GetAsync(url, token);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"Response exception: {(int)response.StatusCode} ({response.ReasonPhrase}) Content:({responseContent})");
                }
                else if (responseContent != null && responseContent.Length > 5)
                {
                    return responseContent;                    
                }

                _logger.LogInformation($"(ThreadId:{Thread.CurrentThread.ManagedThreadId}). Page was got from {url}.");

                return null;
            }
            finally
            {
                response?.Dispose();
            }
        }

        public async Task<string> DownloadFileToLocal(string fileUrl, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return string.Empty;

            HttpResponseMessage response = null;

            try
            {
                _logger.LogInformation($"(ThreadId:{Thread.CurrentThread.ManagedThreadId}). Getting file from url:{fileUrl}");

                response = await _httpClient.GetAsync(fileUrl, token);

                var fileName = Path.GetTempFileName();

                if (response.IsSuccessStatusCode)
                {
                    var fileStream = await response.Content.ReadAsStreamAsync();
                    
                    using (var newFile = File.Create(fileName))
                    {
                        await fileStream.CopyToAsync(newFile, 81920, token);
                    }
                }
                else
                { 
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    throw new HttpRequestException($"Response exception: {(int)response.StatusCode} ({response.ReasonPhrase}) Content:({responseContent})");
                }

                _logger.LogInformation($"(ThreadId:{Thread.CurrentThread.ManagedThreadId}). File was got from {fileUrl}.");

                return fileName;
            }
            finally
            {
                response?.Dispose();
            }
        }

    }
}
