using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinyl.Common;

namespace Vinyl.RecordProcessingJob.Data
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ILogger _logger;
        private readonly IMemoryCache _cacheRates;
        private readonly IHtmlDataGetter _htmlGetter;

        private readonly string _cursesToBynUrl;

        public CurrencyConverter(ILogger<CurrencyConverter> logger, IHtmlDataGetter htmlGetter,
            IMemoryCache memoryCache, string cursesToBynUrl = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _htmlGetter = htmlGetter ?? throw new ArgumentNullException(nameof(htmlGetter));
            _cacheRates = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

            if (_cursesToBynUrl == null)
                _cursesToBynUrl = "http://www.nbrb.by/API/ExRates/Rates/?Periodicity=0";
        }

        public async Task<decimal> ConvertCurrencyToBYN(string currency, decimal? @value)
        {
            if (@value == null || @value == 0)
                return (decimal)0;

            decimal rate;

            // Look for cache key.
            if (!_cacheRates.TryGetValue(currency, out rate))
            {
                rate = await GetOfficialRate(currency);

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromHours(3));

                // Save data in cache.
                _cacheRates.Set(currency, rate, cacheEntryOptions);
            }

            return rate * @value.Value;
        }

        private async Task<decimal> GetOfficialRate(string currency)
        {
            if (string.IsNullOrEmpty(currency))
                return 0;

            try
            {
                var cur = currency.Trim().ToLower();
                var page = await _htmlGetter.GetPage(_cursesToBynUrl);
                var currencies = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CurrencyInfo>>(page) ?? new List<CurrencyInfo>();
                var resItem = currencies.FirstOrDefault(_ => _.Cur_Abbreviation?.Trim().ToLower() == cur);

                return resItem != null
                    ? (resItem.Cur_OfficialRate / (resItem.Cur_Scale > 0 ? resItem.Cur_Scale : 1))
                    : 1;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Error in getting currency rates");
            }

            return 1;
        }

        private class CurrencyInfo
        {
            public int Cur_ID { get; set; }
            public string Cur_Abbreviation { get; set; }
            public string Cur_Name { get; set; }
            public int Cur_Scale { get; set; }
            public decimal Cur_OfficialRate { get; set; }
        }
    }
}
