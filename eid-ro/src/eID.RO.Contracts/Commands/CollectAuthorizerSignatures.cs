using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface CollectAuthorizerSignatures : CorrelatedBy<Guid>
{
    /// <summary>
    /// Id of the empowerment undergoing activation process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// Uids of people that need to sign the empowerment
    /// </summary>
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; }
    /// <summary>
    /// The latest moment where signatures must be collected.
    /// </summary>
    public DateTime SignaturesCollectionDeadline { get; set; }
}
