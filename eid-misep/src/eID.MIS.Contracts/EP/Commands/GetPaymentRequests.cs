using MassTransit;

namespace eID.MIS.Contracts.EP.Commands;

public interface GetPaymentRequests : CorrelatedBy<Guid>
{
    public Guid CitizenProfileId { get; set; }
}
