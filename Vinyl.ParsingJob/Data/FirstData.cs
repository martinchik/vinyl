﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.DbLayer;
using Vinyl.DbLayer.Models;

namespace Vinyl.ParsingJob.Data
{
    static class FirstData
    {
        public static ShopInfo GetLongPlayShop() => new ShopInfo()
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

        public static ShopInfo GetVinylShopShop() => new ShopInfo()
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

        public static ShopInfo GetMuzRayShop() => new ShopInfo()
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

        public static ShopInfo GetTanyaOnlinerShop() => new ShopInfo()
        {
            Id = new Guid("DA33118F-AA27-4FB1-B086-9ECEB56FD0FD"),
            Title = "Колекционер Ta-nya",
            Url = "https://profile.onliner.by/user/203412",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = "",
            Phones = "+375445404005; 3755257604062",
            ShopType = 1,
            ShopParseStrategyInfo = new[]
            {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "TanyaOnlinerPostParserStrategy",
                    StartUrl = "9460502;15404915;9448474;19745827;13887461",
                    ShopId = new Guid("DA33118F-AA27-4FB1-B086-9ECEB56FD0FD"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0,
                    DefaultCurrency = "$"
                }
            }
        };

        public static ShopInfo GetVinPlazaShop() => new ShopInfo()
        {
            Id = new Guid("99F3417C-23D5-4928-BE3F-15F0814AAF07"),
            Title = "VinPlaza",
            Url = "http://www.vinplaza.ru/p/blog-page_4797.html",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = "market@vinplaza.ru",
            Phones = "+37525755З711",
            ShopType = 0,
            ShopParseStrategyInfo = new[]
           {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "VinplazaShopHtmlParserStrategy",
                    StartUrl = "http://www.vinplaza.ru/sitemap.xml?page={0}",
                    ShopId = new Guid("99F3417C-23D5-4928-BE3F-15F0814AAF07"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0,
                    DefaultCurrency = "$"
                }
            }
        };


        public static ShopInfo GetDiscolandShop() => new ShopInfo()
        {
            Id = new Guid("9A11C3EA-11B9-47C0-AC01-8E77E778FB1A"),
            Title = "Discoland",
            Url = "https://discoland.by",
            City = "Minsk",
            CountryCode = "BY",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Emails = " info@discoland.by",
            Phones = "+375296519149",
            ShopType = 0,
            ShopParseStrategyInfo = new[]
           {
                new ShopParseStrategyInfo()
                {
                    Id = Guid.NewGuid(),
                    ClassName = "DiscolandShopHtmlParserStrategy",
                    StartUrl = "https://discoland.by/index.php?categoryID=92&show_all=yes&inst=no;https://discoland.by/index.php?categoryID=91&show_all=yes&inst=no",
                    ShopId = new Guid("9A11C3EA-11B9-47C0-AC01-8E77E778FB1A"),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatePeriodInHours = 12,
                    Status = 0,
                    DefaultCurrency = "бел"
                }
            }
        };
    }
}
