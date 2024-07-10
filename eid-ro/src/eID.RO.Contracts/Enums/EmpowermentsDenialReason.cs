namespace eID.RO.Contracts.Enums;

/// <summary>
/// Denial reason type
/// </summary>
public enum EmpowermentsDenialReason
{
    None = 0,
    DeceasedUid = 1,
    ProhibitedUid = 2,
    NTRCheckFailed = 3,
    TimedOut = 4,
    BelowLawfulAge = 5,
    NoPermit = 7,
    LawfulAgeInfoNotAvailable = 8,
    UnsuccessfulRestrictionsCheck = 9,
    LegalEntityNotActive = 10,
    LegalEntityRepresentationNotMatch = 11,
    UnsuccessfulLegalEntityCheck = 12,
    EmpowermentStatementNotFound = 13,
    BulstatCheckFailed = 14,
    ReregisteredInNTR = 15,
    ArchivedInBulstat = 16,
    InInsolvencyProceedingsInBulstat = 17,
    InsolventInBulstat = 18,
    InLiquidationInBulstat = 19,
    InactiveInBulstat = 20,
    ClosedInBulstat = 21,
    TerminatedThroughMergerInBulstat = 22,
    TerminatedThroughIncorporationInBulstat = 23,
    TerminatedThroughDivisionInBulstat = 24,
    DeletedFromJudicialRegister = 25,
    DeletedFromBTPPRegister = 26,
    RegistrationAnnulledInBulstat = 27,
    TerminatedDueToEnterpriseTransactionInBulstat = 28,
    DeregisteredInBulstat = 29,
    SignatureCollectionTimeOut = 30,
    UnsuccessfulTimestamping = 31,
    DeniedByDeauAdministrator = 32
}
