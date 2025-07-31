using System.Text.Json.Serialization;

namespace eID.PJS.Services.Verification;

public class VerificationServiceStateBase : IPerformanceMetrics
{
    public PerformanceMetric Metrics { get; set; } = new PerformanceMetric();

    public virtual bool IsValid { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public VerificationStatus Status { get; set; }
    public string? Message { get; set; }
    public string ResultLogFile { get; set; }
    public long ResultLogFileSize { get; set; }

}

public class VerificationServiceStateShort : VerificationServiceStateBase
{
    public List<SystemVerificationResultBase> Systems { get; set; } = new List<SystemVerificationResultBase>();
}

public class VerificationServiceState : VerificationServiceStateBase
{
    public List<SystemVerificationResult> Systems { get; set; } = new List<SystemVerificationResult>();
    public List<string> Errors { get; set; } = new List<string> { };

    public override bool IsValid
    {
        get { return Systems.All(r => r.IsValid) && !Errors.Any(); }
    }

    public VerificationServiceStateShort ToShort()
    {
        var result = new VerificationServiceStateShort
        {
            IsValid = this.IsValid,
            Message = this.Message,
            ResultLogFile = this.ResultLogFile,
            ResultLogFileSize = this.ResultLogFileSize,
            Metrics = this.Metrics,
            Status = this.Status,
        };

        foreach (var system in this.Systems)
        {
            result.Systems.Add(new SystemVerificationResultBase
            {
                IndexName = system.IndexName,
                IsValid = system.IsValid,
                LocalLogsLocation = system.LocalLogsLocation,
                ResultLogFile = system.ResultLogFile,
                ResultLogFileSize = system.ResultLogFileSize,
                Status = system.Status,
                SystemId = system.SystemId,
                DatesWithoutLogs = system.DatesWithoutLogs,
                DateOfOldestAvailableRecord = system.DateOfOldestAvailableRecord,
            });
        }

        return result;
    }
}



/// <summary>Verification Status Enum</summary>
public enum VerificationStatus
{
    NotExecuted = 0,
    Finished = 1,
    Canceled = 2,
    CompletedWithErrors = 3,
    ErrorAborted = 4,
    Excluded = 5,
}


