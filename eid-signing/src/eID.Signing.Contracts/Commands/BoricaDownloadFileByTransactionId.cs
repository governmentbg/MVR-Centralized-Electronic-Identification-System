using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface BoricaDownloadFileByTransactionId : CorrelatedBy<Guid>
{
    public string TransactionId { get; set; }
}
