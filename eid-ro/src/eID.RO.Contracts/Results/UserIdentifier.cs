using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

public interface UserIdentifier
{
    string Uid { get; }
    IdentifierType UidType { get; }
}

[Serializable]
public class UserIdentifierData : UserIdentifier
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
}

public interface UserIdentifierWithName
{
    string Uid { get; }
    IdentifierType UidType { get; }
    string Name { get; set; }
    bool IsIssuer { get; }
}

[Serializable]
public class UserIdentifierWithNameData : UserIdentifierWithName
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIssuer { get; set; }
}
