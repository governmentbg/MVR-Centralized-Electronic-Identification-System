#nullable disable
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

public class Provider : 
    AdministratorRegisteredProviderResult, 
    ProviderListResult, 
    ProviderGeneralInformationAndOfficesResult, 
    ProviderInfoResult
{
    public Guid Id { get; set; }
    /// <summary>
    /// Provider registration number. Template: ПДЕАУx/dd.mm.yyyy. x is a integer, dd.mm.yyyy the date of action.
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// When the registration makes from MoI officer,
    /// this field has to be filled with the number from the administrative system
    /// Mandatory only for registration from MoI officer
    /// </summary>
    public string ExternalNumber { get; set; }

    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public string CreatedByAdministratorId { get; set; }
    public string CreatedByAdministratorName { get; set; }
    public string Name { get; set; }
    public string Bulstat { get; set; }
    public string Headquarters { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string IdentificationNumber { get; set; }
    public ProviderStatus Status { get; set; }
    public ProviderType Type { get; set; }
    public DateTime CreatedOn { get; set; }
    public string GeneralInformation { get; set; }
    public string XMLRepresentation { get; set; }

    //AIS Information - not required
    public Guid? AISInformationId { get; set; }
    public virtual AISInformation AISInformation { get; set; }
    public Guid? DetailsId { get; set; }
    public virtual ProviderDetails Details { get; set; }

    //Users
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<ProviderFile> Files { get; set; } = new List<ProviderFile>();
    public virtual ICollection<ProviderStatusHistory> StatusHistory { get; set; } = new List<ProviderStatusHistory>();
    public virtual ICollection<ProviderOffice> Offices { get; set; } = new List<ProviderOffice>();
    public virtual ProviderTimestamp Timestamp { get; set; }
    public virtual ICollection<ProviderDoneService> DoneServices { get; set; } = new List<ProviderDoneService>();

    AISInformationResult ProviderResult.AISInformation { get; set ; }
    IEnumerable<UserResult> ProviderResult.Users { get; set; }
    IEnumerable<ProviderFileResult> ProviderWithFilesResult.Files { get; set; }
    IEnumerable<IProviderOffice> ProviderGeneralInformationAndOfficesResult.Offices { get; set; }
}
#nullable restore
