#nullable disable



using eID;

namespace eID.PJS.Services.Signing
{
    public class MonitoredFolderSettings
    {
        public string MonitorFolder { get; set; }
        public string TargetFolderLogs { get; set; }
        public string TargetFolderHash { get; set; }
    }
}
