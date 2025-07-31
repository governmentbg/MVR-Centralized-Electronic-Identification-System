using eID.PDEAU.Contracts.Enums;

namespace eID.PDEAU.Contracts.Results;

public interface ProviderInfoResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Bulstat { get; set; }
    public string Headquarters { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string IdentificationNumber { get; set; }
    public ProviderType Type { get; set; }
    public DateTime CreatedOn { get; set; }
    public string GeneralInformation { get; set; }
}
