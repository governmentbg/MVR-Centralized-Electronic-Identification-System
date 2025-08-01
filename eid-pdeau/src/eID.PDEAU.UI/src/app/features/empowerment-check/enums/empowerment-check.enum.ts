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
    Empty = 'Empty',
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

export enum CalculatedEmpowermentStatus {
    None = 'None',
    CollectingAuthorizerSignatures = 'CollectingAuthorizerSignatures',
    Active = 'Active',
    Created = 'Created',
    Denied = 'Denied',
    DisagreementDeclared = 'DisagreementDeclared',
    Withdrawn = 'Withdrawn',
    Expired = 'Expired',
    UpComing = 'UpComing',
    Unconfirmed = 'Unconfirmed',
}
