using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using Vinyl.DbLayer.Repository;

namespace Vinyl.DbLayer
{
    public class MetadataRepositoriesFactory : IMetadataRepositoriesFactory
    {
        private readonly ILogger<VinylShopContext> _logger;
        private readonly IConfiguration _configuration;

        public MetadataRepositoriesFactory(IConfiguration configuration, ILogger<VinylShopContext> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }        

        public ShopInfoRepository CreateShopInfoRepository()
            => new ShopInfoRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public ShopParseStrategyInfoRepository CreateShopParseStrategyInfoRepository()
            => new ShopParseStrategyInfoRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public RecordInfoRepository CreateRecordInfoRepository()
            => new RecordInfoRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public RecordInShopLinkRepository CreateRecordInShopLinkRepository()
            => new RecordInShopLinkRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public SearchItemRepository CreateSearchItemRepository()
            => new SearchItemRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public RecordArtRepository CreateRecordArtRepository()
            => new RecordArtRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);

        public RecordLinksRepository CreateRecordLinksRepository()
             => new RecordLinksRepository(DatabaseServiceRegistrator.CreateContext(_configuration), _logger);
    }
}
