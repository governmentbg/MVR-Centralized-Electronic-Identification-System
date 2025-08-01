#nullable disable
using System.ComponentModel.DataAnnotations.Schema;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

public class ProviderService : ProviderServiceResult, ProviderServiceInfoResult
{
    public Guid Id { get; set; }
    public long ServiceNumber { get; set; }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > DBConstraints.ProviderService.NameMaxLength)
            {
                _name = value[..DBConstraints.ProviderService.NameMaxLength];
            }
            else
            {
                _name = value;
            }
        }
    }
    private string _description;
    public string Description
    {
        get { return _description; }
        set
        {
            if (value?.Length > DBConstraints.ProviderService.DescriptionMaxLength)
            {
                _description = value[..DBConstraints.ProviderService.DescriptionMaxLength];
            }
            else
            {
                _description = value;
            }
        }
    }
    /// <summary>
    /// Shows whether or not the service can be empowered
    /// </summary>
    public decimal? PaymentInfoNormalCost { get; set; }
    public bool IsEmpowerment { get; set; }
    /// <summary>
    /// Mark record if it is added from IISDA
    /// </summary>
    public bool SyncedFromOnlineRegistry { get; set; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }
    public ICollection<CollectablePersonalInformation> RequiredPersonalInformation { get; set; } 
    public LevelOfAssurance MinimumLevelOfAssurance { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ReviewerFullName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public ProviderServiceStatus Status { get; set; }
    public string DenialReason { get; set; }
    public DateTime? DeniedOn { get; set; }
    public bool IsActive { get; set; }

    // Foreign keys
    public Guid ProviderDetailsId { get; set; }
    public ProviderDetails ProviderDetails { get; set; }

    public Guid ProviderSectionId { get; set; }
    public ProviderSection ProviderSection { get; set; }

    public ICollection<ServiceScope> ServiceScopes { get; set; }

    public virtual ICollection<ProviderDoneService> DoneServices { get; set; } = new List<ProviderDoneService>();

    IEnumerable<CollectablePersonalInformation> ProviderServiceResult.RequiredPersonalInformation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /// <summary>
    /// Each detail can have only one active provider.
    /// </summary>
    [NotMapped]
    public ProviderResult Provider => ProviderDetails?.Provider;
}
#nullable restore
