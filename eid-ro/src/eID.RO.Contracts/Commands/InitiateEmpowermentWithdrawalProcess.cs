using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface InitiateEmpowermentWithdrawalProcess : CorrelatedBy<Guid>
{
    /// <summary>
    /// If not provided, system-default will be used.
    /// </summary>
    DateTime? WithdrawalsCollectionsDeadline { get; }
    /// <summary>
    /// User who initiated withdrawal operation. EGN or LNCh
    /// </summary>
    string IssuerUid { get; }
    /// <summary>
    /// IssuerUid type
    /// </summary>
    IdentifierType IssuerUidType { get; }
    /// <summary>
    /// Withdraw reason
    /// </summary>
    string Reason { get; }
    /// <summary>
    /// Id of the empowerment undergoing withdraw process
    /// </summary>
    Guid EmpowermentId { get; }
    OnBehalfOf OnBehalfOf { get; }
    IEnumerable<UserIdentifier> AuthorizerUids { get; }
    /// <summary>
    /// Empowered Uids we use in case of successful operation to notify them
    /// </summary>
    IEnumerable<UserIdentifier> EmpoweredUids { get; }
    /// <summary>
    /// Empowerment withdraw record Id
    /// </summary>
    Guid EmpowermentWithdrawalId { get; }
    string LegalEntityUid { get; }
    string LegalEntityName { get; }
    string IssuerName { get; }
    string IssuerPosition { get; }
}
