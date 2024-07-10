using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface DisagreeEmpowerment : CorrelatedBy<Guid>
{
    /// <summary>
    /// User EGN or LNCh got from token
    /// </summary>
    string Uid { get; }

    /// <summary>
    /// Specify EGN or LNCh got from token
    /// </summary>
    IdentifierType UidType { get; }

    /// <summary>
    /// Empowerment Id
    /// </summary>
    Guid EmpowermentId { get; }

    /// <summary>
    /// Disagree reason
    /// </summary>
    string Reason { get; }
}
