using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;
using Vinyl.DbLayer.Repository;
using Vinyl.Metadata;
using Vinyl.RecordProcessingJob.SearchEngine;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class AdditionalInfoSearchEngine : IAdditionalInfoSearchEngine
    {
        private readonly ILogger _logger;
        private readonly IMetadataRepositoriesFactory _metadataFactory;
        private readonly IDiscogsSearchEngine _discogsSearchEngine;

        public AdditionalInfoSearchEngine(ILogger<DirtyRecordImportProcessor> logger, 
            IMetadataRepositoriesFactory metadataFactory,
            IDiscogsSearchEngine discogsSearchEngine)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _discogsSearchEngine = discogsSearchEngine ?? throw new ArgumentNullException(nameof(discogsSearchEngine));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }        

        public bool Search(FindInfosRecord record)
        {
            if (record == null || record.RecordId == Guid.Empty)
                return false;

            try
            {
                return SearchProcess(record.ShopId, record.ShopParseStrategyId, record.RecordId
                        , CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, $"Cannot find additional item for recordid={record.RecordId} and strategyid={record.ShopParseStrategyId}");
            }
            return false;
        }

        private async Task<bool> SearchProcess(Guid shopId, Guid stratefyId, Guid recordId, CancellationToken token)
        {
            using (var recordRepository = _metadataFactory.CreateRecordInfoRepository())
            using (var recordInShopRepository = _metadataFactory.CreateRecordInShopLinkRepository())
            using (var recordLinksRepository = _metadataFactory.CreateRecordLinksRepository())
            using (var recordArtRepository = _metadataFactory.CreateRecordArtRepository())
            {
                var record = recordRepository.Get(recordId);
                if (record == null)
                    return false;

                var allFromShops = recordInShopRepository.FindBy(record.Id, shopId, stratefyId).ToList();
                var barcode = allFromShops
                    .Where(_ => !string.IsNullOrEmpty(_.Barcode))
                    .Select(_ => _.Barcode)
                    .FirstOrDefault();

                var links = await SearchInDiscogs(record, barcode, recordArtRepository, recordLinksRepository, token);
                if (links == null)
                    return false;

                return true;
            }
        }

        private async Task<RecordLinks> SearchInDiscogs(RecordInfo record, string barcode, 
            RecordArtRepository recordArtRepository, RecordLinksRepository recordLinksRepository,
            CancellationToken token)
        { 
            var discogsItem = !string.IsNullOrEmpty(barcode)
                ? await _discogsSearchEngine.FindBy(barcode, token)
                : await _discogsSearchEngine.FindBy(record.Artist, record.Album, record.Year?.ToString(), token);

            await Task.Delay(TimeSpan.FromSeconds(3)); //  Response exception: 429 (Too Many Requests) Content:({"message": "You are making requests too quickly."}

            if (discogsItem != null && !string.IsNullOrEmpty(discogsItem.Value.url))
            {
                var resources = !string.IsNullOrEmpty(discogsItem.Value.resources)
                    ? await _discogsSearchEngine.GetResourcesByUrl(discogsItem.Value.resources, token)
                    : null;

                var tracks = resources?.tracklist?
                        .Where(item => item.title != null && !string.IsNullOrWhiteSpace(item.title))
                        .Select(item => item.title)
                        .ToArray() ?? new string[] { };

                var videos = resources?.videos?
                        .Where(item => item.title != null && !string.IsNullOrWhiteSpace(item.title))
                        .Select(item => string.Concat(item.title, "$",  item.uri))
                        .ToArray() ?? new string[] { };

                if (!string.IsNullOrEmpty(discogsItem.Value.img))
                {
                    recordArtRepository.Add(new RecordArt()
                    {
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Id = Guid.NewGuid(),
                        RecordId = record.Id,
                        PreviewUrl = discogsItem.Value.img,
                        FullViewUrl = string.Empty
                    });

                    await recordArtRepository.CommitAsync();
                }

                var links = new RecordLinks()
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    RecordId = record.Id,
                    ToType = (int)RecordLinkType.Discogs,
                    Link = discogsItem.Value.url,
                    Text = discogsItem.Value.title,
                    Tracks = tracks?.Any() == true ? string.Join("|", tracks) : string.Empty,
                    Videos = videos?.Any() == true ? string.Join("|", videos) : string.Empty
                };

                recordLinksRepository.Add(links);

                await recordLinksRepository.CommitAsync();
                    
                return links;
            }

            return null;           
        }
    }
}
