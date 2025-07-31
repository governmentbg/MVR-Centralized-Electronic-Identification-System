#nullable disable



using System.Collections.Concurrent;

using eID;

namespace eID.PJS.Services.Signing
{
    public class MonitoredFolder : MonitoredFolderSettings, IPerformanceMetrics
    {
        public ConcurrentBag<MonitoredFile> Files { get; set; } = new ConcurrentBag<MonitoredFile>();
        public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();
        public bool HasErrors
        {
            get
            {
                return Files.Any(f => f.Status == MonitoredResourceStatus.Error || f.Error != null);
            }
        }

    }


}
