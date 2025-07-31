namespace eID.PJS.LocalLogsSearch.Service;

public class PerformanceMetric
{
    public TimeSpan ProcessingTime { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ThreadId { get; set; }
    public long PagedMemorySize { get; set; }
    public long PrivateMemorySize { get; set; }
    public long VirtualMemorySize { get; set; }
    public long GCMemorySize { get; set; }
}
