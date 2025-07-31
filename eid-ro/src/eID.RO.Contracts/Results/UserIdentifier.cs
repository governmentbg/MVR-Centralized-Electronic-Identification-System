using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

public interface UserIdentifier
{
    string Uid { get; }
    IdentifierType UidType { get; }
    string Name { get; set; }
}

[Serializable]
public class UserIdentifierData : UserIdentifier
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
}

public interface AuthorizerIdentifier : UserIdentifier
{
    bool IsIssuer { get; }
}

[Serializable]
public class AuthorizerIdentifierData : AuthorizerIdentifier
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsIssuer { get; set; }
}
