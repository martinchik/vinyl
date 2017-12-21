using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;
using Vinyl.Metadata;

namespace Vinyl.RecordProcessingJob.Data
{
    public class RecordService : IRecordService
    {
        private readonly IMetadataRepositoriesFactory _metadataFactory;
        private readonly Regex _regexForNumbers = new Regex(@"^\d$");
        private readonly Regex _regexForDecimals = new Regex(@"[^0-9\.]+");

        public RecordService(IMetadataRepositoriesFactory metadataFactory)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
        }

        public RecordInfo FindOrCreateSimilarBy(DirtyRecord dirtyRecord)
        {
            using (var repository = _metadataFactory.CreateRecordInfoRepository())
            {
                var record = repository.FindBy(dirtyRecord.Artist, dirtyRecord.Album);
                if (record == null)
                {
                    record = new RecordInfo()
                    {
                        Id = Guid.NewGuid(),
                        Title = dirtyRecord.Title?.Trim() ?? string.Empty,
                        Artist = dirtyRecord.Artist?.Trim() ?? string.Empty,
                        Album = dirtyRecord.Album?.Trim() ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (!string.IsNullOrEmpty(dirtyRecord.Year))
                    {                        
                        var nambersOnly = _regexForNumbers.Match(dirtyRecord.Year).Value;
                        if (!string.IsNullOrEmpty(nambersOnly))
                        {
                            int year;
                            if (int.TryParse(nambersOnly, out year))
                                record.Year = year;
                        }
                    }                    

                    repository.Add(record);

                    repository.Commit();
                }

                return record;
            }
        }

        public RecordInShopLink CreateOrUpdateShopLinkInfoBy(DirtyRecord dirtyRecord, RecordInfo record)
        {
            using (var repository = _metadataFactory.CreateRecordInShopLinkRepository())
            {
                var link = repository.FindBy(record.Id, dirtyRecord.ShopId, dirtyRecord.ShopParseStrategyId);
                if (link == null)
                {
                    link = new RecordInShopLink
                    {                        
                        RecordId = record.Id,
                        ShopId = dirtyRecord.ShopId,
                        StrategyId = dirtyRecord.ShopParseStrategyId,
                        CreatedAt = DateTime.UtcNow
                    };                                        
                }

                link.Barcode = dirtyRecord.Barcode;
                link.CountInPack = dirtyRecord.CountInPack;
                link.Country = dirtyRecord.Country;
                link.Label = dirtyRecord.Label;
                link.ShopInfo = dirtyRecord.Info;
                link.ShopUrl = dirtyRecord.Url;
                link.State = dirtyRecord.State?.ToUpper() ?? string.Empty;
                link.Style = dirtyRecord.Style;
                link.ViewType = dirtyRecord.View;
                link.YearRecorded = dirtyRecord.YearRecorded;

                if (!string.IsNullOrEmpty(dirtyRecord.Price))
                {
                    var priceStr = dirtyRecord.Price.Replace(",", ".");
                    var nambersOnly = _regexForDecimals.Replace(priceStr, string.Empty);
                    if (!string.IsNullOrEmpty(nambersOnly))
                    {
                        decimal price;
                        if (decimal.TryParse(nambersOnly, out price))
                            link.Price = price;                        
                    }

                    link.Currency = _regexForDecimals.Match(priceStr).Value.Replace(".", string.Empty).Replace(",", string.Empty).Trim();
                }

                link.UpdatedAt = DateTime.UtcNow;
                if (link.Id == Guid.Empty)
                {
                    link.Id = Guid.NewGuid();
                    repository.Add(link);
                }
                else
                    repository.Update(link);

                repository.Commit();
                return link;
            }
        }
    }
}
