namespace eID.PDEAU.Contracts.Results;

public interface IProviderOffice
{
    string Name { get; set; }
    string Address { get; set; }
    double Lat { get; set; }
    double Lon { get; set; }
}
