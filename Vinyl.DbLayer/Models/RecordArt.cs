using System;
using System.Collections.Generic;

namespace Vinyl.DbLayer.Models
{
    public partial class RecordArt
    {
        public Guid Id { get; set; }
        public Guid RecordId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string PreviewUrl { get; set; }
        public string FullViewUrl { get; set; }
        public RecordInfo Record { get; set; }
    }
}
