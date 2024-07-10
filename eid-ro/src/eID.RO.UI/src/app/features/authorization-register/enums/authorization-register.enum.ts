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
}

export enum IdentifierType {
    EGN = 'EGN',
    LNCh = 'LNCh',
    NotSpecified = 'NotSpecified',
}
