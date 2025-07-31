namespace eID.MIS.Service;

public class IntegrationsModuleRawResponse
{
    public int Status { get; set; }
    public string Error { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
}

public class DataDetails
{
    public Dictionary<string, string[]> Errors { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string TraceId { get; set; }
}
