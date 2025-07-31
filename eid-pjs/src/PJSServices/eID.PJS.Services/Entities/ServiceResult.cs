using System.Net;

namespace eID.PJS.Services;

public class ServiceResult
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

    /// <summary>
    /// Contains all errors usually link with parameters validation messages
    /// </summary>
    public List<KeyValuePair<string, string>>? Errors { get; set; }

    /// <summary>
    /// Only in case of <see cref="HttpStatusCode.InternalServerError"/> this property is filled.
    /// </summary>
    public string? Error { get; set; }
}