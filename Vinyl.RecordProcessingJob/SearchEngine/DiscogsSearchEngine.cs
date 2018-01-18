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
    public class DiscogsSearchEngine
    {
        private readonly string _discogsConsumerKey = "ROWmcrOUwshokHQUIIwR";
        private readonly string _discogsConsumerSecret = "kSMTutfjbzeYmyyWsuDROjTyeyUqxMrH";
        private readonly string _apiUrl = "https://api.discogs.com/";

        private readonly ILogger _logger;
        private readonly IHtmlDataGetter _htmlDataGetter;

        public DiscogsSearchEngine(ILogger<DiscogsSearchEngine> logger, IHtmlDataGetter htmlDataGetter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _htmlDataGetter = htmlDataGetter ?? throw new ArgumentNullException(nameof(htmlDataGetter));
        }

        private AuthenticationHeaderValue BuildAuthHeaderValue()
            => new AuthenticationHeaderValue("Discogs", $"key={_discogsConsumerKey}, secret={_discogsConsumerSecret}");

        public void Initialize()
        {
            
        }

        //https://www.discogs.com/developers/#page:database,header:database-search
        public async Task<dynamic> FindInDiscogsBy(string artist, string album, CancellationToken token)
        {
            var url = $"{_apiUrl}database/search?title={artist}-{album}";
            var result = await _htmlDataGetter.SendAsync(url, BuildAuthHeaderValue(), token);
            if (string.IsNullOrEmpty(result))
                return null;

            var res = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            return res;
        }
    }
}
