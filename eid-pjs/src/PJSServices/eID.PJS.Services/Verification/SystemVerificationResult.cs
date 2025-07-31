using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#nullable disable

namespace eID.PJS.Services.Verification;

public class SystemVerificationResultBase
{ 
    public string SystemId { get; set; }
    public string IndexName { get; set; }
    public string LocalLogsLocation { get; set; }
    public string ResultLogFile { get; set; }
    public long ResultLogFileSize { get; set; }
    public HashSet<DateTime> DatesWithoutLogs = new HashSet<DateTime>();
    public DateTime DateOfOldestAvailableRecord { get; set; }

    public virtual bool IsValid 
    {
        get; set; 
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; }
}

public class SystemVerificationResult: SystemVerificationResultBase
{
    public List<string> Errors { get; set; } = new List<string> { };
    public ConcurrentBag<FileVerificationResult> Files { get; set; } = new ConcurrentBag<FileVerificationResult>();

    public override bool IsValid
    {
        get { return Files.All(r => r.IsValid) && !Errors.Any(); }
    }

}
