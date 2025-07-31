using MassTransit;

namespace eID.MIS.Contracts.EP.Commands;

public interface SuspendPaymentRequestInEPayments : CorrelatedBy<Guid>
{
    public string PaymentRequestId { get; }
}
