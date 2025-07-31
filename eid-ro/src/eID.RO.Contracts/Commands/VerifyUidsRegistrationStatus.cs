using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface VerifyUidsRegistrationStatus : CorrelatedBy<Guid>
{
    /// <summary>
    /// Id of the empowerment undergoing activation process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// Uids of people that need to be verified
    /// </summary>
    public IEnumerable<UserIdentifier> Uids { get; set; }
}
