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
            var shops = await _shopInfoService.GetShops(token);
            long countRecords = 0;

            try
            {
                foreach (var strategy in _strategiesService.GetStrategiesForRun(shops))
                {                   
                    countRecords += await RunStrategy(strategy, token);
                }
            }
            finally
            {
                Result = $"Parsed {countRecords} records";
            }
        }

        private Task<int> RunStrategy(IParserStrategy strategy, CancellationToken token)
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

        //private void GetMessages()
        //{
        //    var msgBus = new Kafka.Lib.MessageBus();
            
        //    Task.Run(() => 
        //        msgBus.SubscribeOnTopic<DirtyRecord>(Kafka.Lib.KafkaConstants.DirtyRecordTopicNameCmd, ProcessKafkaMessage, CancellationToken.None));            
        //}

        //private void ProcessKafkaMessage(DirtyRecord msg)
        //{            
        //    Console.WriteLine($"Recieved msg:{msg.ToString()}");
        //}
    }
}
