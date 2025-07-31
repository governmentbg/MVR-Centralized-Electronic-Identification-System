using MassTransit;

namespace eID.PDEAU.Contracts.Commands;

public interface SetProviderDetailsStatus : CorrelatedBy<Guid>
{
    Guid Id { get; }
    ProviderDetailsStatusType Status { get; }
}

/// <summary>
/// Provider detail status type
/// </summary>
public enum ProviderDetailsStatusType
{
    None = 0,
    Deactivate = 1,
    Activate = 2,
}
