using MassTransit;

namespace eID.MIS.Contracts.EP.Commands;

public interface GetPaymentRequestStatus : CorrelatedBy<Guid>
{
    public Guid PaymentRequestId { get; set; }
    public Guid CitizenProfileId { get; set; }
}
