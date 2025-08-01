namespace eID.PDEAU.Contracts.Results;

public interface ServiceScopeDetailResult
{
    Guid Id { get; set; }
    string Name { get; set; }
    Guid ServiceId { get; }
}
