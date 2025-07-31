using eID.Signing.Contracts.Enums;

namespace eID.Signing.Contracts.Results;

public class BoricaSendConsentResult
{
    public string Code { get; set; }
    public ARSSendConsentResponseData Data { get; set; }
    public string Message { get; set; }
    public ResponseCode? ResponseCode { get; set; }
}

public class ARSSendConsentResponseData
{
    public string CallbackId { get; set; }
    public string Validity { get; set; } // ISO date-time string
}
