namespace eID.PJS.AuditLogging
{
    public interface IAuditLogger
    {
        string? SystemId { get; }

        void LogEvent(AuditLogEvent data);
    }
}