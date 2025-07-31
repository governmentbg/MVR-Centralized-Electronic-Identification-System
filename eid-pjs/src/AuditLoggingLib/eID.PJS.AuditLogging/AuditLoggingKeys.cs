#nullable disable

namespace eID.PJS.AuditLogging;

public static class AuditLoggingKeys
{
    public const string Request = "RequestBody";
    public const string RequesterUserId = nameof(RequesterUserId);
    public const string RequesterUid = nameof(RequesterUid);
    public const string RequesterUidType = nameof(RequesterUidType);
    public const string RequesterName = nameof(RequesterName);
    public const string TargetUserId = nameof(TargetUserId);
    public const string TargetUid = nameof(TargetUid);
    public const string TargetUidType = nameof(TargetUidType);
    public const string TargetName = nameof(TargetName);

    public static string[] GetEncryptablePayloadKeys()
    {
        return new string[] { RequesterUserId, RequesterUid, RequesterName, TargetUserId, TargetUid, TargetName };
    }
}
