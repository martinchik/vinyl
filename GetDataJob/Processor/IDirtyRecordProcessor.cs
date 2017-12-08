using GetDataJob.Model;

namespace GetDataJob.Processor
{
    public interface IDirtyRecordProcessor
    {
        void AddRecord(DirtyRecord record);
    }
}