using System.Diagnostics.CodeAnalysis;
using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public class AuthorizerUidData : IAuthorizerUidData
{
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
}

public interface IAuthorizerUidData
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public string Name { get; set; }
}

public class AuthorizerUidDataEqualityComparer : IEqualityComparer<IAuthorizerUidData>
{
    public bool Equals(IAuthorizerUidData? x, IAuthorizerUidData? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Uid == y.Uid && x.UidType == y.UidType && x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] IAuthorizerUidData obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        // Combine hash codes of Uid and UidType
        int hashUid = obj.Uid.GetHashCode();
        int hashUidType = obj.UidType.GetHashCode();
        int name = obj.Name.GetHashCode();

        unchecked
        {
            // Simple hash code combination algorithm
            return ((hashUid << 5) + hashUid) ^ hashUidType ^ name;
        }
    }
}
