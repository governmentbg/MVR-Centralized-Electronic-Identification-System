using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public class RegisteredSystemTranslation : RegisteredSystemTranslationResult, IEquatable<RegisteredSystemTranslation?>
{
    public string Language { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return Equals(obj as RegisteredSystemTranslation);
    }

    public bool Equals(RegisteredSystemTranslation? other)
    {
        return other is not null &&
               Language == other.Language &&
               Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Language, Name);
    }

    public static bool operator ==(RegisteredSystemTranslation? left, RegisteredSystemTranslation? right)
    {
        return EqualityComparer<RegisteredSystemTranslation>.Default.Equals(left, right);
    }

    public static bool operator !=(RegisteredSystemTranslation? left, RegisteredSystemTranslation? right)
    {
        return !(left == right);
    }
}
