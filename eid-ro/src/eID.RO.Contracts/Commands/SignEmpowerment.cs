using eID.RO.Contracts.Enums;
using MassTransit;

namespace eID.RO.Contracts.Commands;

public interface SignEmpowerment : CorrelatedBy<Guid>
{
    /// <summary>
    /// User name got from token
    /// </summary>
    string Name { get; }
    /// <summary>
    /// User EGN or LNCh got from token
    /// </summary>
    string Uid { get; }

    /// <summary>
    /// Uid type
    /// </summary>
    IdentifierType UidType { get; }

    /// <summary>
    /// Id of empowerment to be signed
    /// </summary>
    Guid EmpowermentId { get; }

    public SignatureProvider SignatureProvider { get; }
    /// <summary>
    /// Base64 encoded detached signature
    /// </summary>
    public string DetachedSignature { get; }
}
