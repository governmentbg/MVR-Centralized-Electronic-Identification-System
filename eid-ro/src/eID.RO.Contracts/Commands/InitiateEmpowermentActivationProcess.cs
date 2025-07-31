using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface InitiateEmpowermentActivationProcess : CorrelatedBy<Guid>
{
    /// <summary>
    /// Id of the empowerment undergoing activation process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// Uid of the individual/legal entity that empowers someone.
    /// </summary>
    public string Uid { get; set; }
    /// <summary>
    /// When OnBehalfOf.LegalEntity the value doesn't matter.
    /// If OnBehalfOf.Individual this is taken from the token.
    /// </summary>
    public IdentifierType UidType { get; set; }
    /// <summary>
    /// Name of the individual/legal entity
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Name of the position the issuer has in the legal entity
    /// </summary>
    public string IssuerPosition { get; set; }
    /// <summary>
    /// Name of the issuer has in the legal entity
    /// </summary>
    public string IssuerName { get; set; }
    /// <summary>
    /// Indicates what's behind Uid 
    /// </summary>
    public OnBehalfOf OnBehalfOf { get; set; }
    /// <summary>
    /// Uids of people that need to sign the empowerment
    /// </summary>
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; }
    /// <summary>
    /// Uids of people that are being empowered
    /// </summary>
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }
    /// <summary>
    /// UTC. When ExpiryDate is set to null the empowerment will be active until withdrawn.
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
}
