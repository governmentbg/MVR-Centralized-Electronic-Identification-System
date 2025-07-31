namespace eID.Signing.Contracts.Results;
public class BoricaFileStatusResult
{
    public string ResponseCode { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public BoricaSignStatusData Data { get; set; }
}

public class BoricaSignStatusData
{
    public IEnumerable<BoricaSignature> Signatures { get; set; }
    public string Cert { get; set; }
}

public class BoricaSignature
{
    public string Signature { get; set; }
    public BoricaSignatureTypeEnum? SignatureType { get; set; }
    public BoricaSignStatusEnum Status { get; set; }
}

public enum BoricaSignatureTypeEnum
{
    CADES_BASELINE_B_DETACHED,
    PADES_BASELINE_B,
    XADES_BASELINE_B_ENVELOPED
}

public enum BoricaSignStatusEnum
{
    ERROR, 
    IN_PROGRESS, 
    SIGNED, 
    RECEIVED, 
    REJECTED, 
    ARCHIVED, 
    REMOVED, 
    EXPIRED
}
