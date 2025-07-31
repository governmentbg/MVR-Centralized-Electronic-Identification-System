namespace eID.PIVR.Contracts.Results;

public interface ApiUsageStatResult
{
    public int Id { get; set; }
    public string RegistryKey { get; set; }
    public DateOnly Date { get; set; }
    public int Count { get; set; }
}
