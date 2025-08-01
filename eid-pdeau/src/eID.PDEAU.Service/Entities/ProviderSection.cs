#nullable disable
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

public class ProviderSection : SectionResult
{
    public Guid Id { get; set; }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > DBConstraints.ProviderSection.NameMaxLength)
            {
                _name = value[..DBConstraints.ProviderSection.NameMaxLength];
            }
            else
            {
                _name = value;
            }
        }
    }
    /// <summary>
    /// Mark record if it is added from IISDA
    /// </summary>
    public bool SyncedFromOnlineRegistry { get; set; }
    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }
    public ICollection<ProviderService> ProviderServices { get; set; }
    public Guid ProviderDetailsId { get; set; }
    public ProviderDetails ProviderDetails { get; set; }
}
#nullable restore
