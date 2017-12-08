namespace GetDataJob.Model
{
    public class DirtyRecord
    {
        public string Url { get; internal set; }
        public string Artist { get; internal set; }
        public string Title { get; internal set; }
        public string Album { get; internal set; }
        public string Info { get; internal set; }
        public string Price { get; internal set; }
        public string Year { get; internal set; }
        public string State { get; internal set; }
        public string Barcode { get; internal set; }

        public override string ToString()
        {
            return string.Concat(Artist, "; ", Album, "; ", Year, "; ", Price, ";", Info);
        }
    }
}