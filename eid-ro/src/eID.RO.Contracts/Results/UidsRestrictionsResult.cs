using eID.RO.Contracts.Enums;

namespace eID.RO.Contracts.Results;

public class UidsRestrictionsResult
{
    /// <summary>
    /// Returns 'true' if only of all of the Authorizers pass the decease and prohibition check
    /// </summary>
    public bool Successful { get; set; }

    /// <summary>
    /// Denial reason in case any of the Authorizers are either prohibited, deceased, underage or without permit.
    /// </summary>
    public EmpowermentsDenialReason DenialReason { get; set; }

    /// <summary>
    /// The field will be filled when there is a <seealso cref="UidsRestrictionsResult.DenialReason"/>
    /// </summary>
    public string DenialReasonDescription { get; set; }
}
