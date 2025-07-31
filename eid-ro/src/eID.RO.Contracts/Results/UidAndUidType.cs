using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

public interface UidAndUidType
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
}

[Serializable]
public class UidAndUidTypeData : UidAndUidType
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
}
