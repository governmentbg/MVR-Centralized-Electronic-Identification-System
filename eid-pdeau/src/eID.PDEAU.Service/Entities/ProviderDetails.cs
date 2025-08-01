#nullable disable
using System.ComponentModel.DataAnnotations.Schema;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;

namespace eID.PDEAU.Service.Entities;

/// <summary>
/// Imported administration of electronic service from IISDA and entered by hand
/// Example: "Министерство на правосъдието"
/// </summary>
public class ProviderDetails : ProviderDetailsResult
{
    public Guid Id { get; set; }
    private string? _identificationNumber;
    public string? IdentificationNumber
    {
        get { return _identificationNumber; }
        set
        {
            if (value?.Length > DBConstraints.IdentificationNumberMaxLength)
            {
                _identificationNumber = value[..DBConstraints.IdentificationNumberMaxLength];
            }
            else
            {
                _identificationNumber = value;
            }
        }
    }
    private string _name = string.Empty;
    public string Name
    {
        get { return _name; }
        set
        {
            if (value.Length > DBConstraints.ProviderDetails.NameMaxLength)
            {
                _name = value[..DBConstraints.ProviderDetails.NameMaxLength];
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
    public ProviderDetailsStatus Status { get; set; }

    private string _uic = string.Empty;
    private string _headquarters;
    private string _address;
    private string _webSiteUrl = string.Empty;
    private string _workingTimeStart = string.Empty;
    private string _workingTimeEnd = string.Empty;

    /// <summary>
    /// Bulstat number. It is usually 13 symbols
    /// </summary>
    public string UIC
    {
        get { return _uic; }
        set
        {
            if (value.Length > DBConstraints.UICMaxLength)
            {
                _uic = value[..DBConstraints.UICMaxLength];
            }
            else
            {
                _uic = value;
            }
        }
    }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }
    public string Headquarters
    {
        get => _headquarters;
        set
        {
            if (value.Length > DBConstraints.HeadquartersLength)
            {
                _headquarters = value[..DBConstraints.HeadquartersLength];
            }
            else
            {
                _headquarters = value;
            }
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            if (value.Length > DBConstraints.AddressLength)
            {
                _address = value[..DBConstraints.HeadquartersLength];
            }
            else
            {
                _address = value;
            }
        }
    }

    public string WebSiteUrl
    {
        get => _webSiteUrl;
        set
        {
            if (value.Length > DBConstraints.ProviderDetails.WebSiteUrlMaxLength)
            {
                _webSiteUrl = value[..DBConstraints.ProviderDetails.WebSiteUrlMaxLength];
            }
            else
            {
                _webSiteUrl = value;
            }
        }
    }

    public string WorkingTimeStart
    {
        get => _workingTimeStart;
        set
        {
            if (value.Length > DBConstraints.ProviderDetails.WorkingTimeStartMaxLength)
            {
                _workingTimeStart = value[..DBConstraints.ProviderDetails.WorkingTimeStartMaxLength];
            }
            else
            {
                _workingTimeStart = value;
            }
        }
    }

    public string WorkingTimeEnd
    {
        get => _workingTimeEnd;
        set
        {
            if (value.Length > DBConstraints.ProviderDetails.WorkingTimeEndMaxLength)
            {
                _workingTimeEnd = value[..DBConstraints.ProviderDetails.WorkingTimeEndMaxLength];
            }
            else
            {
                _workingTimeEnd = value;
            }
        }
    }

    public ICollection<ProviderService> ProviderServices { get; set; }
    public ICollection<Provider> Providers { get; set; }

    /// <summary>
    /// Id of active provider
    /// </summary>
    [NotMapped]
    public Guid? ProviderId { get => Provider?.Id; }

    /// <summary>
    /// Each detail can have only one active provider.
    /// </summary>
    [NotMapped]
    public Provider Provider => Providers?.FirstOrDefault(p => p.Status == ProviderStatus.Active);

    public string GetIdentificationNumber() => IdentificationNumber ?? string.Empty;
}
#nullable restore
