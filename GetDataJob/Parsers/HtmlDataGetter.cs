using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
    }
}
