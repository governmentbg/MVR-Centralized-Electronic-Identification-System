namespace eID.PUN.API.Requests;

public class GetCarriersByFilter
{
    public string SerialNumber { get; set; } = string.Empty;
    public Guid EId { get; set; }
    public Guid CertificateId { get; set; }
}
