using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderStatusHistoryResult
{
    DateTime DateTime { get; set; }
    string ModifierFullName { get; set; }
    ProviderStatus Status { get; set; }
    string Comment { get; set; }
}
