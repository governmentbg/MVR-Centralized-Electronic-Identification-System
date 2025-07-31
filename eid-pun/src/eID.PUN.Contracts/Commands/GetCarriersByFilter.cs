using MassTransit;

namespace eID.PUN.Contracts.Commands;

public interface GetCarriersByFilter : CorrelatedBy<Guid>
{
    string SerialNumber { get; }
    Guid EId { get; }
    Guid CertificateId { get; set; }
    string Type { get; set; }
}
