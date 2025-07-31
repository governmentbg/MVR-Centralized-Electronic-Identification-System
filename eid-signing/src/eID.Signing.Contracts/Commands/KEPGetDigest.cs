using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface KEPGetDigestToSign : KEPGetDigest { }
public interface KEPSignDigest : KEPGetDigest
{
    public string SignatureValue { get; set; }
}


public interface KEPGetDigest : KEPBaseCommand
{
    public string DigestToSign { get; set; }

    public string DocumentName { get; set; }
}
