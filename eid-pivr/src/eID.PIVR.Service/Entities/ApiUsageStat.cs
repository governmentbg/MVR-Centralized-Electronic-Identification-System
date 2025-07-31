using eID.PIVR.Contracts.Results;

namespace eID.PIVR.Service.Entities;

public class ApiUsageStat : ApiUsageStatResult
{
    public int Id { get; set; }
    public string RegistryKey { get; set; }
    public DateOnly Date { get; set; }
    public int Count { get; set; }
}
