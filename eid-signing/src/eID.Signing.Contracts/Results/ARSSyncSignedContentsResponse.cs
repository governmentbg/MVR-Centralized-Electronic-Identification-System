using eID.Signing.Contracts.Enums;

namespace eID.Signing.Contracts.Results;

public class ARSSyncSignedContentsResponse
{
    public string Code { get; set; }
    public RSSyncSignedContentsResponseData Data { get; set; }
    public string Message { get; set; }
    public string ResponseCode { get; set; }

    public class RSSyncSignedContentsResponseData
    {
        public List<ARSSyncSignatureResponse> Signatures { get; set; }

        public class ARSSyncSignatureResponse
        {
            public string Signature { get; set; }
            public string SignatureId { get; set; }
            public SignatureType SignatureType { get; set; }
            public SignatureStatus Status { get; set; }
            public enum SignatureStatus
            {
                ARCHIVED,
                ERROR,
                EXPIRED,
                IN_PROGRESS,
                RECEIVED,
                REJECTED,
                REMOVED,
                REVOKED,
                SIGNED
            }
        }
    }
}
