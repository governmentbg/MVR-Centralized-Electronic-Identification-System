using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface EvrotrustCheckUserByUid : CorrelatedBy<Guid>
{
    public string Uid { get; set; }
}
