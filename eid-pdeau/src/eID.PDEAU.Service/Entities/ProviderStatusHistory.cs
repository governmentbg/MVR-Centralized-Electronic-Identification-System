#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

public class ProviderStatusHistory : ProviderStatusHistoryResult
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public string ModifierUid { get; set; }
    public IdentifierType ModifierUidType { get; set; }
    public string ModifierFullName { get; set; }
    public ProviderStatus Status { get; set; }
    /// <summary>
    /// Optional
    /// </summary>
    public string Comment { get; set; }

    // Foreign key
    public Guid ProviderId { get; set; }
    public Provider Provider { get; set; }
}
#nullable restore
