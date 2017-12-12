using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common.Extensions;
using Vinyl.Common.Job;
using System.Threading;
using Vinyl.ParsingJob.Processor;
using Vinyl.ParsingJob.Data;
using Vinyl.ParsingJob.Parsers;
using Vinyl.Metadata;

namespace Vinyl.ParsingJob.Job
{
    public class ParsingRepeatableJob : RepeatableJob
    {
        private readonly IShopStrategiesService _strategiesService;
        private readonly IShopInfoService _shopInfoService;
        private readonly IDirtyRecordProcessor _recordProcessor;

        public const string Name = "parsing-job";

        public ParsingRepeatableJob(ILogger<ParsingRepeatableJob> logger, IShopStrategiesService strategiesService, IShopInfoService shopInfoService,
            IDirtyRecordProcessor recordProcessor) :
            base(logger, Name, Repeat.Immediately().Than(Repeat.Each.Hours(3)), TimeSpan.FromHours(1))
        {
            _strategiesService = strategiesService ?? throw new ArgumentNullException(nameof(strategiesService));
            _shopInfoService = shopInfoService ?? throw new ArgumentNullException(nameof(shopInfoService));
            _recordProcessor = recordProcessor ?? throw new ArgumentNullException(nameof(recordProcessor));            
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            //Task.Run(() =>
            //        new Kafka.Lib.MessageBus(Kafka.KafkaConstants.DirtyRecordTopicNameCmd, "127.0.0.1:9092")
            //        .SubscribeOnTopic<DirtyRecord>(msg => {
            //            Logger.LogInformation($"Recieved msg:{msg.ToString()}");
            //        }, token),
            //    token);

            var shops = await _shopInfoService.GetShops(token);

            Logger.LogInformation($"Get {shops?.Count} shops from storage");

            long countRecords = 0;
            Result = "";
            try
            {
                await Task.WhenAll(_strategiesService
                    .GetStrategiesForRun(shops)
                    .Select(strategyInfo => Task.Run(() =>
                    {
                        var count = RunStrategy(strategyInfo, token); ;
                        Interlocked.Add(ref countRecords, count);
                    }))
                    .ToArray());
            }
            catch (Exception exc)
            {
                Result += "Error:"+ exc.Message + Environment.NewLine;
                throw exc;
            }
            finally
            {
                Result += $"Parsed and sent {countRecords} records" + Environment.NewLine;
            }
        }

        private int RunStrategy((ShopParseStrategyInfo info, IParserStrategy strategy) strategyPair, CancellationToken token)
        {
            int count = 0;
            try
            {
                Logger.LogInformation($"Run {strategyPair.Item1.ClassName} parsing strategy with id={strategyPair.Item1.Id}");

                foreach (var record in strategyPair.strategy.Parse(token))
                {
                    if (_recordProcessor.AddRecord(strategyPair.info, record))
                        count++;
                }
            }
            catch (Exception exc)
            {
                var txt = $"Error while running {strategyPair.Item1.ClassName} parsing strategy with id={strategyPair.Item1.Id}";

                Result += txt + ". Error:" + exc.Message + Environment.NewLine;
                Logger.LogWarning(exc, txt);
            }

            return count;
        }
    }
}
