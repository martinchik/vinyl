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
    }
}