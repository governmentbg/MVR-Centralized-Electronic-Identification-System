using MassTransit;

namespace eID.PAN.Contracts.Commands;

public interface GetSystemById : CorrelatedBy<Guid>
{
    public Guid Id { get; }
}
