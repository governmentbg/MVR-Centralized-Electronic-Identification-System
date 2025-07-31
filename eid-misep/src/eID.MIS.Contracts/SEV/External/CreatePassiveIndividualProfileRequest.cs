namespace eID.MIS.Contracts.SEV.External;

public class CreatePassiveIndividualProfileRequest
{
    public string Identifier { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public AddressData Address { get; set; }
}
