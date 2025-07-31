using eID.MIS.Contracts.EP.External;
using MassTransit;

namespace eID.MIS.Contracts.EP.Commands;

public interface CreatePaymentRequest : CorrelatedBy<Guid>
{
    public Guid CitizenProfileId { get; set; }
    public RegisterPaymentRequest Request { get; set; }
    public string SystemName { get; set; }
}
