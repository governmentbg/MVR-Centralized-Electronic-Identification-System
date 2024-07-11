namespace eID.PAN.Contracts.Results;

public interface TranslationResult
{
    public string Language { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
}
