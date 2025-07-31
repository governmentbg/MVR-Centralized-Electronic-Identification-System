namespace eID.Signing.Contracts.Results;

public class BoricaFileContent
{
    public string ResponseCode { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public object Content { get; set; }
}
