using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface CheckLegalEntityInNTR : CorrelatedBy<Guid>
{
    /// <summary>
    /// Id of the empowerment undergoing activation process
    /// </summary>
    public Guid EmpowermentId { get; set; }
    /// <summary>
    /// Uid of the legal entity that empowers someone.
    /// </summary>
    public string Uid { get; set; }
    /// <summary>
    /// Name of the legal entity
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Must be present in the result from NTR
    /// </summary>
    public string IssuerUid { get; set; }
    /// <summary>
    /// IssuerUid type
    /// </summary>
    public IdentifierType IssuerUidType { get; set; }
    /// <summary>
    /// Must be exact match with the result from NTR
    /// </summary>
    public string IssuerName { get; set; }
    /// <summary>
    /// Must be exact match with the result from NTR
    /// </summary>
    public string IssuerPosition { get; set; }
}

public class CheckLegalEntityInNTRData : CheckLegalEntityInNTR
{
    public Guid EmpowermentId { get; set; }
    public string Uid { get; set; }
    public string Name { get; set; }
    public string IssuerUid { get; set; }
    public IdentifierType IssuerUidType { get; set; }
    public string IssuerName { get; set; }
    public string IssuerPosition { get; set; }
    public Guid CorrelationId { get; set; }
}
