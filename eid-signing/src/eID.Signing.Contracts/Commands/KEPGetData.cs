using MassTransit;

namespace eID.Signing.Contracts.Commands;

public interface KEPGetDataToSign : KEPGetData { }
public interface KEPSignData : KEPGetData 
{
    public string SignatureValue { get; set; }
}


public interface KEPGetData : KEPBaseCommand
{
    public string DocumentToSign { get; set; }
}
