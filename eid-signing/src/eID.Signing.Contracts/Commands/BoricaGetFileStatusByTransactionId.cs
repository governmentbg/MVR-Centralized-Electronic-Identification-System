using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaGetFileStatusByTransactionId : CorrelatedBy<Guid>
{
    public string TransactionId { get; set; }
}
