using eID.RO.Contracts.Enums;

namespace eID.RO.Service.Responses;

public class LegalEntityBulstatVerificationResult
{
    public bool Successful { get; set; } = false;

    public EmpowermentsDenialReason DenialReason { get; set; } = EmpowermentsDenialReason.None;
}
