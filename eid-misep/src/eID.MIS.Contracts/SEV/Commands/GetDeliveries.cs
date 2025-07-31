using MassTransit;

namespace eID.MIS.Contracts.SEV.Commands;

public interface GetDeliveries : CorrelatedBy<Guid>
{
    public Guid EIdentityId { get; set; }
}
