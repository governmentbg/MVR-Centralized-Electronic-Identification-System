namespace eID.PDEAU.API.Responses;

public class RegiXSearchResult<T> where T : class
{
    public IDictionary<string, T> Response { get; set; }
    public bool HasFailed { get; set; }
    public string? Error { get; set; }
}
