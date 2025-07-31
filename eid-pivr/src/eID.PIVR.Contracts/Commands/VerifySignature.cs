using eID.PIVR.Contracts.Enums;
using MassTransit;

namespace eID.PIVR.Contracts.Commands;

public interface VerifySignature : CorrelatedBy<Guid>
{
    /// <summary>
    /// Original file as text
    /// </summary>
    string OriginalFile { get; }

    /// <summary>
    /// Base64 encoded detached signature
    /// </summary>
    string DetachedSignature { get; }

    /// <summary>
    /// Citizen EGN or LNCh
    /// </summary>
    string Uid { get; }

    /// <summary>
    /// Uid type EGN or LNCh
    /// </summary>
    IdentifierType UidType { get; }

    SignatureProvider SignatureProvider { get; }
}
