using eID.Signing.Contracts.Enums;

namespace eID.Signing.Contracts.Results;

public class BoricaCheckConsentStatusResponse
{
    public string Code { get; set; }
    public RSGetConsentResponseData Data { get; set; }
    public string Message { get; set; }
    public ResponseCode ResponseCode { get; set; }
}

public class RSGetConsentResponseData
{
    public string AccessToken { get; set; }
    public string Consent { get; set; }
    public DateTime ExpirationDate { get; set; }
}
