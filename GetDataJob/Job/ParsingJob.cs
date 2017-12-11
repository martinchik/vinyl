using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common.Extensions;
using Vinyl.Common.Job;
using System.Threading;
using Vinyl.GetDataJob.Processor;
using Vinyl.GetDataJob.Data;
using Vinyl.GetDataJob.Parsers;

namespace Vinyl.GetDataJob.Job
{
    public class ParsingJob : RepeatableJob
    {
        private readonly IShopStrategiesService _strategiesService;
        private readonly IShopInfoService _shopInfoService;

        public const string Name = "parsing-job";

        public ParsingJob(ILogger logger, IShopStrategiesService strategiesService, IShopInfoService shopInfoService) :
            base(logger, Name, Repeat.Immediately().Than(Repeat.Each.Hours(3)), TimeSpan.FromHours(1))
        {
            _strategiesService = strategiesService ?? throw new ArgumentNullException(nameof(strategiesService));
            _shopInfoService = shopInfoService ?? throw new ArgumentNullException(nameof(shopInfoService));
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var shops = await _shopInfoService.GetShops(token);

            foreach (var strategy in _strategiesService.GetStrategiesForRun(shops))
            {
                await RunStrategy(strategy, token);
            }
        }

        private Task RunStrategy(IParserStrategy strategy, CancellationToken token)
        {
            try
            {
                return strategy.Run(token);
            }
            catch (Exception exc)
            {
                Logger.LogWarning(exc, $"Error while running {strategy.GetType().Name} parsing strategy ");
                return Task.FromResult(0);
            }
        }
    }
}
