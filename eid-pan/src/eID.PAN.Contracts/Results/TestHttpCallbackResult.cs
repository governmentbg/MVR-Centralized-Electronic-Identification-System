using System.Net;

namespace eID.PAN.Contracts.Results;

public interface TestHttpCallbackResult
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
    public string Response { get; set; }
}


