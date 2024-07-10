using System.Diagnostics.CodeAnalysis;
using eID.RO.Contracts.Results;

namespace eID.RO.Service;

public class UserIdentifierEqualityComparer : IEqualityComparer<UserIdentifier>
{
    public bool Equals(UserIdentifier? x, UserIdentifier? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Uid == y.Uid && x.UidType == y.UidType;
    }

    public int GetHashCode([DisallowNull] UserIdentifier obj)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        // Combine hash codes of Uid and UidType
        int hashUid = obj.Uid.GetHashCode();
        int hashUidType = obj.UidType.GetHashCode();

        unchecked
        {
            // Simple hash code combination algorithm
            return ((hashUid << 5) + hashUid) ^ hashUidType;
        }
    }
}

public class UserIdentifierWithNamesEqualityComparer : IEqualityComparer<UserIdentifierWithName>
{
    public bool Equals(UserIdentifierWithName? x, UserIdentifierWithName? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        return x.Uid == y.Uid && x.UidType == y.UidType && x.Name == y.Name;
    }

    public int GetHashCode([DisallowNull] UserIdentifierWithName obj)
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

