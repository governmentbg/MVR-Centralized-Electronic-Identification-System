#nullable disable



using eID;

namespace eID.PJS.Services.Signing
{
    public class MonitoredFile: IPerformanceMetrics
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public long Size { get; set; }
        public MonitoredResourceStatus Status { get; set; }
        public Exception Error { get; set; }

        public string FullName
        {
            get
            {
                return Path.Combine(Folder, Name);
            }
        }

        public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();
    }


}
