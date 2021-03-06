﻿using Microsoft.EntityFrameworkCore;
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
        
        public SearchItem GetBy(Guid recordId, string countryCode)
        {
            if (recordId == Guid.Empty)
                return null;

            return Context.SearchItem.SingleOrDefault(t => t.RecordId == recordId && t.CountryCode == countryCode);
        }

        public SearchItem GetBy(string recordUrl, string countryCode)
        {
            if (string.IsNullOrEmpty(recordUrl))
                return null;

            return Context.SearchItem.SingleOrDefault(t => t.RecordUrl == recordUrl && t.CountryCode == countryCode);
        }

        public IQueryable<SearchItem> Find(string words, string countryCode)
        {
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
