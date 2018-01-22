using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Vinyl.RecordProcessingJob.SearchEngine
{
    public class YoutubeSearchEngine
    {
        private readonly string _key = "AIzaSyDUhMttyX3p1jW5M9cBMyxcZoA5GEnV-XQ";
        private readonly ILogger _logger;

        public YoutubeSearchEngine(ILogger<YoutubeSearchEngine> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(string[] videos, string[] channels, string[] playlists)> FindBy(string artist, string album, string year, CancellationToken token)
        {
            var textToSearch = $"{artist} {album}".AddIfExist(" ", year);

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _key,
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = textToSearch; // Replace with your search term.
            searchListRequest.MaxResults = 3;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync(token);

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            return (videos.ToArray(), channels.ToArray(), playlists.ToArray());
        }
    }
}
