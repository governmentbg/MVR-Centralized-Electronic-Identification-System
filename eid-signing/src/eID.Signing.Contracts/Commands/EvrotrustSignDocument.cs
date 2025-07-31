using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface EvrotrustSignDocument : CorrelatedBy<Guid>
{
    public DateTime DateExpire { get; set; }
    public IEnumerable<EvrotrustDocumentToSign> Documents { get; set; }
    public IEnumerable<string> UserIdentifiers { get; set; }
}
public interface EvrotrustDocumentToSign
{
    public string Content { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
}
