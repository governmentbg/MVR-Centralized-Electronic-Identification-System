namespace eID.PIVR.Application.Options;

public class RegiXOptions
{
    public string ServiceUrl { get; set; } = string.Empty;
    public int ConcurrentMessageLimit { get; set; } = 1;
    public int SocketsHttpHandlerConnectTimeoutInMs { get; set; } = 200;
    public bool UseProductionService { get; set; }
}
