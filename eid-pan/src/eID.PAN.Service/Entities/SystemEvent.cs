using eID.PAN.Contracts.Results;
using Newtonsoft.Json;

namespace eID.PAN.Service.Entities;

public class SystemEvent : SystemEventResult, IEquatable<SystemEvent?>
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsDeleted { get; set; }

    [JsonIgnore]
    public Guid RegisteredSystemId { get; set; }
    [JsonIgnore]
    public RegisteredSystem RegisteredSystem { get; set; }

    public ICollection<Translation> Translations { get; set; } = new List<Translation>();

    public ICollection<DeactivatedUserEvent> DeactivatedUserEvent { get; set; } = new List<DeactivatedUserEvent>();

    IEnumerable<TranslationResult> SystemEventResult.Translations { get => Translations; set => throw new NotImplementedException(); }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SystemEvent);
    }

    public bool Equals(SystemEvent? other)
    {
        return other is not null &&
               //Id.Equals(other.Id) && // We need to check only content
               Code == other.Code &&
               IsMandatory == other.IsMandatory &&
               IsDeleted == other.IsDeleted &&
               Translations is not null &&
               other.Translations is not null &&
               !other.Translations.Except(Translations).Any();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, IsMandatory, IsDeleted, Translations);
    }

    public static bool operator ==(SystemEvent? left, SystemEvent? right)
    {
        return EqualityComparer<SystemEvent>.Default.Equals(left, right);
    }

    public static bool operator !=(SystemEvent? left, SystemEvent? right)
    {
        return !(left == right);
    }
}
