#nullable disable
using eID.PAN.Contracts.Results;

namespace eID.PAN.Service.Entities;

public class RegisteredSystem : RegisteredSystemResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsApproved { get; set; }
    /// <summary>
    /// True when archived
    /// </summary>
    public bool IsDeleted { get; set; }
    public ICollection<RegisteredSystemTranslation> Translations { get; set; } = new List<RegisteredSystemTranslation>();

    public ICollection<SystemEvent> Events { get; set; } = new List<SystemEvent>();

    IEnumerable<SystemEventResult> RegisteredSystemResult.Events { get => Events; set => throw new NotImplementedException(); }
    IEnumerable<RegisteredSystemTranslationResult> RegisteredSystemResult.Translations { get => Translations; set => throw new NotImplementedException(); }
}

public class RegisteredSystemRejected : RegisteredSystemRejectedResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime RejectedOn { get; set; }
    public string RejectedBy { get; set; }
    public ICollection<RegisteredSystemTranslation> Translations { get; set; } = new List<RegisteredSystemTranslation>();
    public string Reason { get; set; }

    IEnumerable<RegisteredSystemTranslationResult> RegisteredSystemRejectedResult.Translations { get => Translations; set => throw new NotImplementedException(); }
}
#nullable restore
