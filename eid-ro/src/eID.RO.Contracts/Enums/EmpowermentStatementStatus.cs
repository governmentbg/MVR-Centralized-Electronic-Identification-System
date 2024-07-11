namespace eID.RO.Contracts.Enums;

public enum EmpowermentStatementStatus
{
    None = 0,
    /// <summary>
    /// Awaiting AuthorizerIds for their signatures
    /// </summary>
    CollectingAuthorizerSignatures = 1,
    /// <summary>
    /// Process of withdrawal is started.
    /// </summary>
    [Obsolete("This status won't be used in the future")]
    CollectingWithdrawalSignatures = 2,
    /// <summary>
    /// All checks passed successfully and all signatures are collected.
    /// </summary>
    Active = 3,
    /// <summary>
    /// Newly created empowerments
    /// </summary>
    Created = 4,
    /// <summary>
    /// Some checks failed
    /// </summary>
    Denied = 5,
    /// <summary>
    /// One or more empowered people disagreed with the empowerment.
    /// </summary>
    DisagreementDeclared = 6,
    /// <summary>
    /// All withdrawal signatures were collected.
    /// </summary>
    Withdrawn = 7,
    /// <summary>
    /// Unclear form of Legal Entity's empowerment representation
    /// </summary>
    Unconfirmed = 8
}
