using eID.PDEAU.Contracts.Enums;
using static eID.PDEAU.Contracts.Constants;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderServiceResult
{
    Guid Id { get; set; }
    long ServiceNumber { get; set; }
    string Name { get; set; }
    string? Description { get; set; }
    decimal? PaymentInfoNormalCost { get; set; }
    /// <summary>
    /// Shows whether or not the service can be empowered
    /// </summary>
    bool IsEmpowerment { get; set; }
    /// <summary>
    /// Mark record if it is added from IISDA
    /// This field is ignored in EF
    /// </summary>
    bool SyncedFromOnlineRegistry { get ; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    bool IsDeleted { get; set; }
    public IEnumerable<CollectablePersonalInformation> RequiredPersonalInformation { get; set; }
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
    Guid ProviderDetailsId { get; }
    Guid ProviderSectionId { get; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public ProviderServiceStatus Status { get; set; }
    public string DenialReason { get; set; }
    public DateTime? DeniedOn { get; set; }
    public bool IsActive { get; set; }
    public ProviderResult Provider { get;}
}
