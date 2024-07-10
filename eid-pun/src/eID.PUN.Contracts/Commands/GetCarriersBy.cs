using MassTransit;

namespace eID.PUN.Contracts.Commands;

public interface GetCarriersBy : CorrelatedBy<Guid>
{
    public string SerialNumber { get; }
    public Guid EId { get; }
    public Guid CertificateId { get; set; }
}
