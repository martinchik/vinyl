﻿using System;
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
        private readonly ICurrencyConverter _currencyConverter;

        public RecordService(IMetadataRepositoriesFactory metadataFactory, ICurrencyConverter currencyConverter)
        {
            _metadataFactory = metadataFactory ?? throw new ArgumentNullException(nameof(metadataFactory));
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
        }

        public RecordInfo FindOrCreateSimilarBy(DirtyRecord dirtyRecord, out bool isNew)
        {
            isNew = false;
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

                    record.Year = ParseSpecialFields.ParseYear(dirtyRecord.Year);
                    record.UpdatedAt = DateTime.UtcNow;
                    repository.Add(record);

                    repository.Commit();
                    isNew = true;
                }

                return record;
            }
        }

        public RecordInShopLink UpdateOrCreateShopLinkInfoBy(DirtyRecord dirtyRecord, RecordInfo record, Metadata.ShopParseStrategyInfo strategy, out bool hasImportantChanges)
        {
            hasImportantChanges = false;
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

                var state = ParseSpecialFields.ParseState(dirtyRecord.State);
                if (link.State != state)
                {
                    hasImportantChanges = true;
                    link.State = state;
                }

                link.Style = dirtyRecord.Style;
                link.ViewType = dirtyRecord.View;
                link.YearRecorded = dirtyRecord.YearRecorded;

                var price = ParseSpecialFields.ParsePrice(dirtyRecord.Price, strategy?.DefaultCurrency);
                if (price.price != null && price.price != link.Price &&
                    !string.IsNullOrEmpty(price.currency) && price.currency != link.Currency)
                {
                    hasImportantChanges = true;

                    link.Price = price.price;
                    link.Currency = price.currency;
                    link.PriceBy = _currencyConverter.ConvertCurrencyToBYN(price.currency, price.price).GetAwaiter().GetResult();
                }

                link.UpdatedAt = DateTime.UtcNow;
                if (link.Id == Guid.Empty)
                {
                    link.Id = Guid.NewGuid();
                    repository.Add(link);
                    hasImportantChanges = true;
                }
                else
                    repository.Update(link);

                repository.Commit();
                return link;
            }
        }

        public bool UpdateOrCreateSearchItem(RecordInfo record, bool isNewRecord, bool hasImportantChanges)
        {
            if (record == null)
                return false;

            using (var repository = _metadataFactory.CreateSearchItemRepository())
            using (var linksRepository = _metadataFactory.CreateRecordInShopLinkRepository())
            {
                SearchItem searchItem = null;
                if (!isNewRecord)
                {
                    searchItem = repository.GetBy(record.Id);
                    if (!hasImportantChanges)
                        return false;
                }
                if (searchItem == null)
                {
                    searchItem = new SearchItem();
                    searchItem.RecordId = record.Id;
                }

                var linkPrices = linksRepository.FindBy(record.Id).Select(_ => new
                {
                    _.Price,
                    _.PriceBy,
                    _.State,
                    _.ShopId,
                    isActive = _.Strategy != null ? _.Strategy.Status == (int)StrategyStatus.Active : false
                }).ToList();

                searchItem.PriceFrom = linkPrices.Select(_ => _.PriceBy > 0 ? _.PriceBy : _.Price ?? 0).Min();
                searchItem.PriceTo = linkPrices.Select(_ => _.PriceBy > 0 ? _.PriceBy : _.Price ?? 0).Max();
                searchItem.Sell = linkPrices.All(_ => !_.isActive) ? false : true;
                searchItem.TextLine1 = $"{record.Artist} / {record.Album}".AddIfExist(" / ", record.Year?.ToString());
                searchItem.TextLine2 = $"В {linkPrices.Where(_ => _.ShopId != Guid.Empty).Distinct().Count()} магазинах".AddIfExist(" в состоянии (", string.Join(",", linkPrices.Where(_ => !string.IsNullOrEmpty(_.State)).Select(_ => _.State)),")");

                if (searchItem.Id == Guid.Empty)
                {
                    searchItem.Id = Guid.NewGuid();
                    repository.Add(searchItem);
                }
                else
                    repository.Update(searchItem);

                repository.Commit();
            }

            return true;
        }

    }
}