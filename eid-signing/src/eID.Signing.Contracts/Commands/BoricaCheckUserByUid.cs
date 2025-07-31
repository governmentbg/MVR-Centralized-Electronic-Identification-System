using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaCheckUserByUid : CorrelatedBy<Guid>
{
    public string Uid { get; set; }
}
