namespace eID.PDEAU.Contracts.Results;

public interface ProviderServiceInfoResult
{
    public Guid Id { get; set; }
    public long ServiceNumber { get; set; }

    public string Name {  get; set; }
    public string Description { get; set; }
    /// <summary>
    /// Shows whether or not the service can be empowered
    /// </summary>
    public decimal? PaymentInfoNormalCost { get; set; }
}
