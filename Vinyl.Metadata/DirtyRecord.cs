namespace Vinyl.Metadata
{
    public class DirtyRecord
    {
        public string Url { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public string Info { get; set; }
        public string Price { get; set; }
        public string Year { get; set; }
        public string State { get; set; }
        public string Barcode { get; set; }
        public string Country { get; set; }
        public string CountInPack { get; set; }
        public string YearRecorded { get; set; }
        public string Label { get; set; }
        public string Style { get; set; }
        public string View { get; set; } //LP, СLP

        public override string ToString()
        {
            return string.Concat(Artist, "; ", Album, "; ", Year, "; ", Price, ";", Info, ";");
        }
    }
}