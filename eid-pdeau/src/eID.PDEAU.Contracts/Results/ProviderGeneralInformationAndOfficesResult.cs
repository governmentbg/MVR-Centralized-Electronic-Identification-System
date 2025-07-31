namespace eID.PDEAU.Contracts.Results;

public interface ProviderGeneralInformationAndOfficesResult
{
    string GeneralInformation { get; set; }
    IEnumerable<IProviderOffice> Offices { get; set; }
}
