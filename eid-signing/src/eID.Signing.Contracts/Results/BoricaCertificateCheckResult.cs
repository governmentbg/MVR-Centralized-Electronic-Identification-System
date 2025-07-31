namespace eID.Signing.Contracts.Results;
public class BoricaCertificateCheckResult
{
    public string ResponseCode { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public CertificateData Data { get; set; }
}

public class CertificateData
{
    public string CertReqId { get; set; }
    public IEnumerable<string> Devices { get; set; }
    public string EncodedCert { get; set; }
}
