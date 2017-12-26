using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.RecordProcessingJob.Processor
{
    public class AdditionalInfoSearchEngine : IAdditionalInfoSearchEngine
    {
        private BlockingCollection<Guid> _itemsToSearch;

        private readonly ILogger _logger;
        private readonly IMetadataRepositoriesFactory _metadataFactory;

        public AdditionalInfoSearchEngine(ILogger<DirtyRecordImportProcessor> logger, IMetadataRepositoriesFactory metadataFactory)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _itemsToSearch = new BlockingCollection<Guid>(1000);

            Task.Run(() =>
            {
                foreach (var nextUuid in _itemsToSearch.GetConsumingEnumerable())
                {
                    SearchProcess(nextUuid);
                }
            });
        }

        private void SearchProcess(Guid nextUuid)
        {
            try
            {
                using (var repository = _metadataFactory.CreateRecordInfoRepository())
                {
                    var ri = repository.Get(nextUuid);
                    if (ri != null)
                    {
                        _logger.LogInformation("SearchProcess started search info for " + ri.ToString());
                        SearchProcess(ri);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "SearchProcess has error");
            }
        }

        public void AddToSearchQueue(Guid recordId)
        {
            if (recordId == Guid.Empty)
                return;

            _itemsToSearch.Add(recordId);
        }

        private void SearchProcess(RecordInfo ri)
        {

        }

    }
}
