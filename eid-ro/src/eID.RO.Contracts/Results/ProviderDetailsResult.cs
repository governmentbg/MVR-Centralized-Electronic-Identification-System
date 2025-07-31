using eID.RO.Contracts.Commands;

namespace eID.RO.Contracts.Results;

public interface ProviderListResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
