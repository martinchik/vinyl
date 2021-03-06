﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class RecordLinksRepository : BaseRepositoryTemplate<RecordLinks>
    {
        internal RecordLinksRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.RecordLinks, logger)
        {
        }
        
        public IEnumerable<RecordLinks> FindBy(Guid recordId, int linkType)
        {            
            return Set.Where(_ => 
                _.RecordId == recordId &&
                _.ToType == linkType);
        }

        public int RemoveIfExists(Guid recordId, int linkType)
        {
            var res = FindBy(recordId, linkType).ToList();

            foreach (var item in res)
                Set.Remove(item);

            return res.Count;
        }
    }
}
