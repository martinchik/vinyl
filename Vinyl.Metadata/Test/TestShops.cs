﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Vinyl.Metadata.Test
{
    public static class TestShops
    {
        public static ShopInfo[] GetAll() => new[]
        {
            GetLongPlayShop(),
            GetVinylShopShop()
        };

        public static ShopInfo GetLongPlayShop() => new ShopInfo()
        {
            Id = new Guid("191b1839-e4fc-4787-b178-34e0cb5bcb53"),
            Name = "LongPlay",
            Url = "http://longplay.by/",
            Strategies = new[] 
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "LongPlayHtmlParserStrategy",
                    Url = "http://longplay.by/vse-stili.html?ditto_111_display=96&ditto_111_sortBy=pagetitle&ditto_111_sortDir=ASC&111_start={0}&111_start={1}",
                    ShopId = new Guid("191b1839-e4fc-4787-b178-34e0cb5bcb53"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),     
                    DataLimit = 100,
                    UpdatePeriodInHours = 12
                }
            }
        };

        public static ShopInfo GetVinylShopShop() => new ShopInfo()
        {
            Id = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
            Name = "VinylShop",
            Url = "http://www.vinylshop.by/",
            Strategies = new[]
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopHtmlParserStrategy",
                    Url = "http://www.vinylshop.by/products/page/{0}/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    DataLimit = 100,
                    UpdatePeriodInHours = 12
                },
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopExcelParserStrategy",
                    Url = "http://www.vinylshop.by/2015/10/%D1%81%D0%BE%D0%B2%D1%80%D0%B5%D0%BC%D0%B5%D0%BD%D0%BD%D1%8B%D0%B5-%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D1%80%D0%BE%D1%81%D1%81%D0%B8%D0%B9%D1%81%D0%BA%D0%B8%D0%B5-%D0%B8%D1%81/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    DataLimit = 100,
                    UpdatePeriodInHours = 12,
                    Parameters = new Dictionary<string, string>
                    {
                        { "class-name", "post-content" },
                        { "ref-link-text","СКАЧАТЬ" }
                    }
                },
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinylShopMMExcelParserStrategy",
                    Url = "http://www.vinylshop.by/2015/07/%D0%BF%D0%BB%D0%B0%D1%81%D1%82%D0%B8%D0%BD%D0%BA%D0%B8-%D0%BC%D0%B8%D1%80%D1%83%D0%BC%D0%B8%D1%80/",
                    ShopId = new Guid("a4210836-824b-4b7b-bb29-996fcb53cf73"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    DataLimit = 100,
                    UpdatePeriodInHours = 12,
                    Parameters = new Dictionary<string, string>
                    {
                        { "class-name", "post-content" },
                        { "ref-link-text", "Скачать каталог" }
                    }
                }
            }
        };
    }
}
