using eID.MIS.Contracts.SEV.External;

namespace eID.MIS.Contracts.SEV.Results;

public interface GetProfileResult
{
    public string ProfileId { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public AddressData Address { get; set; }
}
