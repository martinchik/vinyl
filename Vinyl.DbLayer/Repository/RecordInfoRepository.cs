using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Vinyl.DbLayer.Models;

namespace Vinyl.DbLayer.Repository
{
    public class RecordInfoRepository : BaseRepositoryTemplate<RecordInfo>
    {
        internal RecordInfoRepository(VinylShopContext context, ILogger<VinylShopContext> logger)
            :base(context, context.RecordInfo, logger)
        {
        }

        protected override bool OnUpdateValidation(RecordInfo item)
        {
            if (string.IsNullOrWhiteSpace(item.Artist))
                throw new ArgumentNullException(nameof(item.Artist));
            if (string.IsNullOrWhiteSpace(item.Album))
                throw new ArgumentNullException(nameof(item.Album));

            var other = FindBy(item.Artist, item.Album);
            if (other != null && other.Id != item.Id)
                throw new Exception($"Record '{item.Artist}' - '{item.Album}' is already exist");

            return true;
        }

        public RecordInfo FindBy(string artist, string album)
        {
           
            return Context.RecordInfo.FirstOrDefault(_ => 
                string.Compare(_.Artist, artist, true) == 0 &&
                string.Compare(_.Album, album, true) == 0);
        }

        public RecordInfo GetFull(Guid id)
        {
            return Context.RecordInfo
                .Include(_ => _.RecordArt)
                .Include(_ => _.RecordInShopLink)
                .Include(_ => _.RecordLinks)
                .FirstOrDefault(_ => _.Id == id);
        }
    }
}
