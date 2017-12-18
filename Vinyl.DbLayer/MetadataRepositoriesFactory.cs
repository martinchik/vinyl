using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
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
        {
            var ctx = DatabaseServiceRegistrator.CreateContext(_configuration);
            return new ShopInfoRepository(ctx, _logger);
        }
    }
}
