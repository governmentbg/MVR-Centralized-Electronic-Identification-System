using eID.Signing.Contracts.Enums;

namespace eID.Signing.Contracts.Results;

public class BoricaValidateConsentTokenResponse
{
    public string Code { get; set; }
    public string Message { get; set; }
    public ResponseCode ResponseCode { get; set; }
}
