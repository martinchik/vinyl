using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class RecordInfo
    {
        public RecordInfo()
        {
            RecordArt = new HashSet<RecordArt>();
            RecordInShopLink = new HashSet<RecordInShopLink>();
            RecordLinks = new HashSet<RecordLinks>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int? Year { get; set; }
        public string Info { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<RecordArt> RecordArt { get; set; }
        public ICollection<RecordInShopLink> RecordInShopLink { get; set; }
        public ICollection<RecordLinks> RecordLinks { get; set; }
    }
}
