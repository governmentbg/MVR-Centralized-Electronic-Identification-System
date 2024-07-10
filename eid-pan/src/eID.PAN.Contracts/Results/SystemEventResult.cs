namespace eID.PAN.Contracts.Results;

public interface SystemEventResult
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string ModifiedBy { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsDeleted { get; set; }

    public IEnumerable<TranslationResult> Translations { get; set; }
}
