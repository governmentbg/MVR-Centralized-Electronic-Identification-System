using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface EvrotrustGetFileStatusByTransactionId : CorrelatedBy<Guid>
{
    public string TransactionId { get; set; }
    public bool GroupSigning { get; set; }
}
