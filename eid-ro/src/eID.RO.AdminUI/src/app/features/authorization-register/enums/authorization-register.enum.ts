export enum EmpowermentStatementStatus {
    None = 'None',
    // "Подпиши"
    CollectingAuthorizerSignatures = 'CollectingAuthorizerSignatures',
    // "Активно"
    Active = 'Active',
    // "Изтекло"
    Expired = 'Expired',
    // "В обработка"
    Created = 'Created',
    // "Отказано"
    Denied = 'Denied',
    // "Декларирано несъгласие"
    DisagreementDeclared = 'DisagreementDeclared',
    // "Оттеглено"
    Withdrawn = 'Withdrawn',
    // "Непотвърдено"
    Unconfirmed = 'Unconfirmed',
}

export enum SortOrder {
    'desc' = -1,
    'asc' = 1,
}

export enum OnBehalfOf {
    LegalEntity = 'LegalEntity',
    Individual = 'Individual',
}

export enum TypeOfEmpowerment {
    Separately = 0,
    Together = 1,
}

export enum EmpowermentsDenialReason {
    None = 'None',
    DeceasedAuthorizer = 'DeceasedAuthorizer',
    ProhibitedAuthorizer = 'ProhibitedAuthorizer',
    NTRCheckFailed = 'NTRCheckFailed',
    TimedOut = 'TimedOut',
    BelowLawfulAge = 'BelowLawfulAge',
    EmpoweredBelowLawfulAge = 'EmpoweredBelowLawfulAge',
    EmpoweredLawfulAgeInfoNotAvailable = 'EmpoweredLawfulAgeInfoNotAvailable',
    DeceasedUid = 'DeceasedUid',
    ProhibitedUid = 'ProhibitedUid',
    NoPermit = 'NoPermit',
    UnsuccessfulRestrictionsCheck = 'UnsuccessfulRestrictionsCheck',
    LegalEntityNotActive = 'LegalEntityNotActive',
    LegalEntityRepresentationNotMatch = 'LegalEntityRepresentationNotMatch',
    UnsuccessfulLegalEntityCheck = 'UnsuccessfulLegalEntityCheck',
    EmpowermentStatementNotFound = 'EmpowermentStatementNotFound',
    BulstatCheckFailed = 'BulstatCheckFailed',
    ReregisteredInNTR = 'ReregisteredInNTR',
    ArchivedInBulstat = 'ArchivedInBulstat',
    InInsolvencyProceedingsInBulstat = 'InInsolvencyProceedingsInBulstat',
    InsolventInBulstat = 'InsolventInBulstat',
    InLiquidationInBulstat = 'InLiquidationInBulstat',
    InactiveInBulstat = 'InactiveInBulstat',
    ClosedInBulstat = 'ClosedInBulstat',
    TerminatedThroughMergerInBulstat = 'TerminatedThroughMergerInBulstat',
    TerminatedThroughIncorporationInBulstat = 'TerminatedThroughIncorporationInBulstat',
    TerminatedThroughDivisionInBulstat = 'TerminatedThroughDivisionInBulstat',
    DeletedFromJudicialRegister = 'DeletedFromJudicialRegister',
    DeletedFromBTPPRegister = 'DeletedFromBTPPRegister',
    RegistrationAnnulledInBulstat = 'RegistrationAnnulledInBulstat',
    TerminatedDueToEnterpriseTransactionInBulstat = 'TerminatedDueToEnterpriseTransactionInBulstat',
    DeregisteredInBulstat = 'DeregisteredInBulstat',
    UnsuccessfulTimestamping = 'UnsuccessfulTimestamping',
    SignatureCollectionTimeOut = 'SignatureCollectionTimeOut',
    DeniedByDeauAdministrator = 'DeniedByDeauAdministrator',
    InvalidEmpoweredUidRegistrationStatusDetected = 'InvalidEmpoweredUidRegistrationStatusDetected',
    EmpoweredUidsRegistrationStatusInfoNotAvailable = 'EmpoweredUidsRegistrationStatusInfoNotAvailable',
}

export enum IdentifierType {
    EGN = 'EGN',
    LNCh = 'LNCh',
}

export enum DocumentType {
    ID_CARD = 'ID_CARD',
    PASSPORT = 'PASSPORT',
    PASSPORT2 = 'PASSPORT2',
}

export enum DeviceType {
    MOBILE = 'MOBILE',
    IDENTITY_CARD = 'IDENTITY_CARD',
}
