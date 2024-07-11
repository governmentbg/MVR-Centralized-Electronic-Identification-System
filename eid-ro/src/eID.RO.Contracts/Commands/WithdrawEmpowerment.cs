using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface WithdrawEmpowerment : CorrelatedBy<Guid>
{
    /// <summary>
    /// User EGN or LNCh got from token
    /// </summary>
    string Uid { get; }

    /// <summary>
    /// Uid type
    /// </summary>
    IdentifierType UidType { get; set; }

    /// <summary>
    /// Empowerment Id
    /// </summary>
    Guid EmpowermentId { get; }

    /// <summary>
    /// Withdraw reason
    /// </summary>
    string Reason { get; }
    
    /// <summary>
    /// Full name of the user got from token
    /// </summary>
    string Name { get; }
}
