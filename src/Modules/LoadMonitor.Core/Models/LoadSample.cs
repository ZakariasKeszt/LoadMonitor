namespace LoadMonitor.Core.Models
{
    public class LoadSample
    {
        public long ID { get; set; }
        public string Profile { get; set; }
        public int Sequence { get; set; }
        public DateTime Timestamp { get; set; }
        public double PowerW { get; set; }
    }
}
