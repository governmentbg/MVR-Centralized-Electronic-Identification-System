namespace eID.PDEAU.Service.Responses;

public class PivrProviderResponse
{
    public int PageIndex { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<PivrProviderData> Data { get; set; }
}
