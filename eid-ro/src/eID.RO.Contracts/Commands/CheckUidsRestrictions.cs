using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface CheckUidsRestrictions : CorrelatedBy<Guid>
{
    /// <summary>
    /// Id of the empowerment undergoing activation process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// Uids of people that signed the empowerment
    /// </summary>
    public IEnumerable<UserIdentifier> Uids { get; set; }
    /// <summary>
    /// In cases of call relying on a response we need to return fast response
    /// </summary>
    public bool RapidRetries { get; set; }
    /// <summary>
    /// If we want to be more specific with our responses. We can ask for raw ServiceResult and handle it
    /// </summary>
    public bool RespondWithRawServiceResult { get; set; }
}
