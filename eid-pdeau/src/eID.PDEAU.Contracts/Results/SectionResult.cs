namespace eID.PDEAU.Contracts.Results;

public interface SectionResult
{
    Guid Id { get; set; }
    string Name { get; set; }
    bool SyncedFromOnlineRegistry { get; set; }
    bool IsDeleted { get; set; }
}
