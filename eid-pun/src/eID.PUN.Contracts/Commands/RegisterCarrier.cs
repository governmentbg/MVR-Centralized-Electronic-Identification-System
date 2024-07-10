using MassTransit;

namespace eID.PUN.Contracts.Commands;

public interface RegisterCarrier : CorrelatedBy<Guid>
{
    public string SerialNumber { get; set; }
    public string Type { get; set; }
    public Guid CertificateId { get; set; }
    public Guid EId { get; set; }
    public Guid UserId { get; set; }
    public string ModifiedBy { get; set; }
}
