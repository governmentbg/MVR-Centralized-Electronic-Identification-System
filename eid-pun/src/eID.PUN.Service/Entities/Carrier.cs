using eID.PUN.Contracts.Results;

namespace eID.PUN.Service.Entities;

public class Carrier : CarrierResult
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; }
    public string Type { get; set; }
    public Guid CertificateId { get; set; }
    public Guid EId { get; set; }
    public DateTime? ModifiedOn { get; set; }
   
}
