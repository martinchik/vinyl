﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer
{
    class MetadataInitializer
    {    
        public MetadataInitializer ClearData(VinylShopContext context)
        {
            if (context.ShopInfo.Any())
            {
                context.Database.ExecuteSqlCommand(@"
                    Delete from ""SearchItem"";
                    Delete from ""RecordArt"";
                    Delete from ""RecordLinks"";
                    Delete from ""RecordInShopLink"";
                    Delete from ""RecordInfo"";
                    Delete from ""ShopParseStrategyInfo"";
                    Delete from ""ShopInfo"";
                    DROP FUNCTION fts_search;
                    DROP INDEX idx_fts_resords;
                    DROP FUNCTION make_tsvector;
                ");
                context.SaveChanges(true);
            }

            return this;
        }

        public MetadataInitializer RestartParsing(VinylShopContext context)
        {
            foreach (var it in context.ShopParseStrategyInfo.ToArray())
            {
                it.LastProcessedCount = 0;
                it.ProcessedAt = DateTime.UtcNow.AddDays(-1);
                context.ShopParseStrategyInfo.Update(it);
            }
            context.SaveChanges(true);
            return this;
        }

        public MetadataInitializer Initialize(VinylShopContext context)
        {
            var shops = context.ShopInfo.ToList();
            if (!shops.Any())
            {
                context.ShopInfo.Add(GetLongPlayShop());
                context.ShopInfo.Add(GetVinylShopShop());
                context.ShopInfo.Add(GetMuzRayShop());

                context.SaveChanges(true);
            }
            else 
            {
                var msh = GetMuzRayShop();
                if (!shops.Any(_ => _.Id == msh.Id))
                {
                    context.ShopInfo.Add(msh);
                    context.SaveChanges(true);
                }
            }

            return this;
        }

        private static ShopInfo GetLongPlayShop() => new ShopInfo()
        {
            Id = new Guid("191b1839-e4fc-4787-b178-34e0cb5bcb53"),
            Title = "LongPlay",
            Url = "http://longplay.by/",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = "beduin-in@mail.ru",
            Phones = "+375296172345; +375297688898",
            ShopParseStrategyInfo = new[]
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "LongPlayHtmlParserStrategy",
                    StartUrl = "http://longplay.by/vse-stili.html?ditto_111_display=96&ditto_111_sortBy=pagetitle&ditto_111_sortDir=ASC&111_start={0}&111_start={1}",
                    ShopId = new Guid("191b1839-e4fc-4787-b178-34e0cb5bcb53"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0
                }
            }
        };

        private static ShopInfo GetVinylShopShop() => new ShopInfo()
        {
            Id = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
            Title = "VinylShop",
            Url = "http://www.vinylshop.by/",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = "vinylshoporder@gmail.com",
            Phones = "",
            ShopParseStrategyInfo = new[]
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopHtmlParserStrategy",
                    StartUrl = "http://www.vinylshop.by/products/page/{0}/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0
                },
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopExcelParserStrategy",
                    StartUrl = "http://www.vinylshop.by/2015/10/%D1%81%D0%BE%D0%B2%D1%80%D0%B5%D0%BC%D0%B5%D0%BD%D0%BD%D1%8B%D0%B5-%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D1%80%D0%BE%D1%81%D1%81%D0%B8%D0%B9%D1%81%D0%BA%D0%B8%D0%B5-%D0%B8%D1%81/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Parameters = new Dictionary<string, string>
                    {
                        { "class-name", "post-content" },
                        { "ref-link-text","СКАЧАТЬ" }
                    }.ToParametersDbString(),
                    Status = 1,
                    DefaultCurrency = "rub"
                },
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopExcelParserStrategy",
                    StartUrl = "http://www.vinylshop.by/2015/10/%D1%81%D0%BE%D0%B2%D1%80%D0%B5%D0%BC%D0%B5%D0%BD%D0%BD%D1%8B%D0%B5-%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D0%B7%D0%B0%D0%BF%D0%B0%D0%B4/",
                    ShopId = new Guid("936845B7-9976-4C7A-A4AC-2D8019C6EEA1"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = null,
                    UpdatePeriodInHours = 12,
                    Parameters = new Dictionary<string, string>
                    {
                        { "class-name", "post-content" },
                        { "ref-link-text","СКАЧАТЬ" }
                    }.ToParametersDbString(),
                    Status = 1,
                    DefaultCurrency = "rub"
                },
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopMMExcelParserStrategy",
                    StartUrl = "http://www.vinylshop.by/2015/07/%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D0%BC%D0%B8%D1%80%D1%83%D0%BC%D0%B8%D1%80/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Parameters = new Dictionary<string, string>
                    {
                        { "class-name", "post-content" },
                        { "ref-link-text", "Скачать каталог" }
                    }.ToParametersDbString(),
                    Status = 0,
                    DefaultCurrency = "rub"
                }
            }
        };

        private static ShopInfo GetMuzRayShop() => new ShopInfo()
        {
            Id = new Guid("4BDB7592-41FB-43E2-879A-FD94013ECA77"),
            Title = "Музыкальный Рай",
            Url = "http://muzraiminsk.business.site/",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = "kozlova.maria.92@gmail.com",
            Phones = "+375293242424; +375336242424",
            ShopParseStrategyInfo = new[]
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylMuzRayGoogleExcelParserStrategy",
                    StartUrl = "1rF7EpvUTVqqY8UoXK6FQERCQzTZYUfxH",
                    ShopId = new Guid("4BDB7592-41FB-43E2-879A-FD94013ECA77"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0
                }
            }
        };
    }
}
