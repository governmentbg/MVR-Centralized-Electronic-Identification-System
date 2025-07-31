using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#nullable disable

namespace eID.PJS.Services.Signing
{
    public class SigningServiceState : IPerformanceMetrics
    {
        public ConcurrentBag<MonitoredFolder> Folders { get; set; } = new ConcurrentBag<MonitoredFolder>();
        public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();
        public int NumberOfThreadsUsed => CalculateNumberOfThreadsUsed();
        public bool HasErrors => Folders.Any(f => f.HasErrors);
        public int NumSkippedFiles => Folders.Sum(f => f.Files.Count(c => c.Status == MonitoredResourceStatus.Skipped));
        public int NumProcessedFiles => Folders.Sum(f => f.Files.Count(c => c.Status == MonitoredResourceStatus.Processed));
        public int NumErrors => Folders.Sum(f => f.Files.Count(c => c.Status == MonitoredResourceStatus.Error));
        private int CalculateNumberOfThreadsUsed()
        {
            var lst = new HashSet<int>();
            lst.Add(Metrics.ThreadId);

            Folders.ToList().ForEach(f =>
            {
                lst.Add(f.Metrics.ThreadId);
                f.Files.ToList().ForEach(l => lst.Add(l.Metrics.ThreadId));
            });

            return lst.Count;
        }
    }

    public class SigningServiceStateRecord : IPerformanceMetrics
    {
        public List<MonitoredFolder> Folders { get; set; } = new List<MonitoredFolder>();
        public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();
        public int NumberOfThreadsUsed { get; set; }
        public bool HasErrors  { get; set; }
        public int NumSkippedFiles { get; set; }
        public int NumProcessedFiles { get; set; }
        public int NumErrors { get; set; }
       
    }
}
