using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class RecordArtRepository : BaseRepositoryTemplate<RecordArt>
    {
        internal RecordArtRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.RecordArt, logger)
        {
        }
        
        public RecordArt FindBy(Guid recordId)
        {            
            return Set.FirstOrDefault(_ => 
                _.RecordId == recordId);
        }

        public string FindPreview(Guid recordId)
        {
            return Set.Where(_ => _.RecordId == recordId).Select(_ => _.PreviewUrl).FirstOrDefault();
        }

        public string FindFullImage(Guid recordId)
        {
            return Set
                .Where(_ => _.RecordId == recordId)
                .Select(_ => _.FullViewUrl ?? _.PreviewUrl)
                .FirstOrDefault();
        }
    }
}
