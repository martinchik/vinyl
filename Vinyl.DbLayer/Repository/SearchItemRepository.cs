using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class SearchItemRepository : BaseRepositoryTemplate<SearchItem>
    {
        internal SearchItemRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.SearchItem, logger)
        {
        }        

        public SearchItem GetBy(Guid recordId)
        {
            if (recordId == Guid.Empty)
                return null;

            return Context.SearchItem.SingleOrDefault(t => t.RecordId == recordId);
        }

        public SearchItem GetBy(Guid recordId, string countryCode)
        {
            if (recordId == Guid.Empty)
                return null;

            return Context.SearchItem.SingleOrDefault(t => t.RecordId == recordId && t.CountryCode == countryCode);
        }

        public IQueryable<SearchItem> Find(string text, string countryCode)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 3)
                return Enumerable.Empty<SearchItem>().AsQueryable();

            var words = Regex.Replace(text.Trim().ToLower(), @"\s+", ",");
            var sqlParameterWords = new NpgsqlParameter("@words", words);
            var sqlParameterCountry = new NpgsqlParameter("@country", countryCode);

            return Context
                .SearchItem
                .FromSql("SELECT * FROM fts_search(@words, @country)", sqlParameterWords, sqlParameterCountry)
                .AsNoTracking()
                .AsQueryable();
        }
    }
}
