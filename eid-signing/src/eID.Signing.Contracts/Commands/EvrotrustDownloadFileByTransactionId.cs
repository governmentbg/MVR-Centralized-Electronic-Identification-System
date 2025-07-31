using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface EvrotrustDownloadFileByTransactionId : CorrelatedBy<Guid>
{
    public string TransactionId { get; set; }
    public bool GroupSigning { get; set; }
}
