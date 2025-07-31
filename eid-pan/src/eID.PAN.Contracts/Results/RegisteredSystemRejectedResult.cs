namespace eID.PAN.Contracts.Results;

public interface RegisteredSystemRejectedResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime RejectedOn { get; set; }
    public string RejectedBy { get; set; }
    public IEnumerable<RegisteredSystemTranslationResult> Translations { get; set; }
    public string Reason { get; set; }
}
