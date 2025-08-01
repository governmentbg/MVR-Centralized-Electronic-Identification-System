using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderDetailsResult
{
    Guid Id { get; set; }
    string IdentificationNumber { get; set; }
    string Name { get; set; }
    bool SyncedFromOnlineRegistry { get; }
    ProviderDetailsStatus Status { get; set; }
    string UIC { get; set; }
    string Address { get; set; }
    string Headquarters { get; set; }
}
