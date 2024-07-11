using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public class Translation : TranslationResult, IEquatable<Translation?>
{ 
    public string Language { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return Equals(obj as Translation);
    }

    public bool Equals(Translation? other)
    {
        return other is not null &&
               Language == other.Language &&
               ShortDescription == other.ShortDescription &&
               Description == other.Description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Language, ShortDescription, Description);
    }

    public static bool operator ==(Translation? left, Translation? right)
    {
        return EqualityComparer<Translation>.Default.Equals(left, right);
    }

    public static bool operator !=(Translation? left, Translation? right)
    {
        return !(left == right);
    }
}
