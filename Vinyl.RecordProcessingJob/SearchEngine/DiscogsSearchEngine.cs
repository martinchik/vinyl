using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.Common;

namespace Vinyl.RecordProcessingJob.SearchEngine
{
    //https://www.discogs.com/developers/#page:database,header:database-search
    public class DiscogsSearchEngine : IDiscogsSearchEngine
    {
        private readonly string _discogsConsumerKey = "ROWmcrOUwshokHQUIIwR";
        private readonly string _discogsConsumerSecret = "kSMTutfjbzeYmyyWsuDROjTyeyUqxMrH";
        private readonly string _apiUrl = "https://api.discogs.com/";
        private readonly string _siteUrl = "https://www.discogs.com/";

        private readonly ILogger _logger;
        private readonly IHtmlDataGetter _htmlDataGetter;

        public DiscogsSearchEngine(ILogger<DiscogsSearchEngine> logger, IHtmlDataGetter htmlDataGetter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
        }

        private AuthenticationHeaderValue BuildAuthHeaderValue()
            => new AuthenticationHeaderValue("Discogs", $"key={_discogsConsumerKey}, secret={_discogsConsumerSecret}");
        
        public async Task<(string title, string img, string url, long id)?> FindBy(string barcode, CancellationToken token)
        {
            var url = $"{_apiUrl}database/search?barcode={barcode}";

            return await FindByUrl(url, token);
        }

        public async Task<(string title, string img, string url, long id)?> FindBy(string artist, string album, string year, CancellationToken token)
        {
            var url = $"{_apiUrl}database/search?q={artist}-{album}&{{?title,artist}}";
            if (!string.IsNullOrEmpty(year))
                url += "&year=" + year;
            
            return await FindByUrl(url, token);
        }

        private async Task<(string title, string img, string url, long id)?> FindByUrl(string url, CancellationToken token)
        {
            var result = await _htmlDataGetter.SendAsync(url, BuildAuthHeaderValue(), token);
            if (string.IsNullOrEmpty(result))
                return null;

            try
            {
                var resValue = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                if (resValue != null)
                {
                    if (resValue.pagination?.items > 0)
                    {
                        var items = resValue.results;
                        foreach (var item in items)
                        {
                            if (item.type == "master" || item.type == "release")
                            {
                                return (
                                    title: item.title ?? string.Empty,
                                    img: item.thumb ?? string.Empty,
                                    url: item.uri != null ? string.Concat(_siteUrl, (string)item.uri) : string.Empty,
                                    id: item.id ?? (long)0
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Error in parsing results from discogs");
            }

            return null;
        }
    }
}
