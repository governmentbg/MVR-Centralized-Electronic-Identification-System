namespace eID.RO.Service.Responses;

public class TimestampResponse
{
    public string Code { get; set; }
    public SigningData Data { get; set; }
    public string Message { get; set; }
    public string ResponseCode { get; set; }
}

public class SigningData
{
    public List<SignatureInfo> Signatures { get; set; }
}

public class SignatureInfo
{
    public string Signature { get; set; }
    public string SignatureId { get; set; }
    public string SignatureType { get; set; }
    public string Status { get; set; }
}
