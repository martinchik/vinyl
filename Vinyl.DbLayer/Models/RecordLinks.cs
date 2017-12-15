using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class RecordLinks
    {
        public Guid Id { get; set; }
        public Guid RecordId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ToType { get; set; }
        public string Link { get; set; }
        public string Text { get; set; }

        public RecordInfo Record { get; set; }
    }
}
