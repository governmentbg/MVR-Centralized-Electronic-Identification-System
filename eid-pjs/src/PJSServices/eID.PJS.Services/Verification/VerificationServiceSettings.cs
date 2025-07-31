using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenSearch.Net;

#nullable disable

namespace eID.PJS.Services.Verification;

public class VerificationServiceSettings : StatefulServiceSettingsBase
{
    public Dictionary<string, SystemLocations> Systems { get; set; } = new Dictionary<string, SystemLocations>();

    public string AuditLogFileExtension { get; set; } = "*.audit";
    public string HashFileExtension { get; set; } = "*.hash";
    public VerificationCheckPeriod VerifyPeriod { get; set; }
    public TimeSpan VerifyPeriodOffset { get; set; } = TimeSpan.FromHours(1); // Use default offset of one hour to skip the latest logs that are not yet redy for verification.
    public bool UseLocalTime { get; set; }
    public string CommandStateProviderType { get; set; } = typeof(InMemoryCommandStateProvider<VerificationServiceSettings>).AssemblyQualifiedName;
}

public enum VerificationCheckPeriod
{ 
    Disabled = 0,
    All = 1,
    Week = 2,
    TwoWeeks = 3,
    Month = 4,
    Quarter = 5,
    Year = 6,
    Day = 7

}

public class SystemLocations
{ 
    public string AuditLogs { get; set; }
    public string Hashes { get; set; }

}

