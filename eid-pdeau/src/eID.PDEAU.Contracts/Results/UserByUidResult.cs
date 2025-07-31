using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface UserByUidResult
{
    public string Uid { get; set; }
    public IdentifierType UidType { get; set; }
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; }
    public bool IsAdministrator { get; set; }
}
