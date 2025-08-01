#nullable disable
using System.ComponentModel.DataAnnotations.Schema;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

public partial class ServiceScope : ServiceScopeResult, ServiceScopeDetailResult
{
    public Guid Id { get; set; }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > DBConstraints.ServiceScope.NameMaxLength)
            {
                _name = value[..DBConstraints.ServiceScope.NameMaxLength];
            }
            else
            {
                _name = value;
            }
        }
    }

    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }

    // Foreign key
    public Guid ProviderServiceId { get; set; }
    public ProviderService ProviderService { get; set; }
    [NotMapped]
    public Guid ServiceId => ProviderServiceId;
}
#nullable restore
