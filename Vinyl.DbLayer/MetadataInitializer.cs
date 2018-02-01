using Microsoft.EntityFrameworkCore;
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
                it.ProcessedAt = DateTime.UtcNow.AddDays(-1);
                context.ShopParseStrategyInfo.Update(it);
            }
            context.SaveChanges(true);
            return this;
        }        
    }
}
