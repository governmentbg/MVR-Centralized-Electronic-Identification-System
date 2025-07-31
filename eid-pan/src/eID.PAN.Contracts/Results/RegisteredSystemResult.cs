namespace eID.PAN.Contracts.Results;

public interface RegisteredSystemResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public bool IsApproved { get; set; }
    public bool IsDeleted { get; set; }
    public IEnumerable<RegisteredSystemTranslationResult> Translations { get; set; }

    public IEnumerable<SystemEventResult> Events { get; set; }
}
