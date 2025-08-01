export enum PersonalIdTypes {
    EGN = 'EGN',
    LNCH = 'LNCh',
}

export enum ApplicationStatus {
    SUBMITTED = 'SUBMITTED',
    PROCESSING = 'PROCESSING',
    PENDING_SIGNATURE = 'PENDING_SIGNATURE',
    SIGNED = 'SIGNED',
    PENDING_PAYMENT = 'PENDING_PAYMENT',
    PAID = 'PAID',
    DENIED = 'DENIED',
    APPROVED = 'APPROVED',
    GENERATED_CERTIFICATE = 'GENERATED_CERTIFICATE',
    COMPLETED = 'COMPLETED',
    CERTIFICATE_STORED = 'CERTIFICATE_STORED',
    PAYMENT_EXPIRED = 'PAYMENT_EXPIRED',
}

export enum CertificateStatus {
    CREATED = 'CREATED',
    ACTIVE = 'ACTIVE',
    STOPPED = 'STOPPED',
    REVOKED = 'REVOKED',
    FAILED = 'FAILED',
    EXPIRED = 'EXPIRED',
}

export enum DeviceType {
    MOBILE = 'MOBILE',
    CHIP_CARD = 'CHIP_CARD',
    OTHER = 'OTHER',
}

export enum LevelOfAssurance {
    LOW = 'LOW',
    SUBSTANTIAL = 'SUBSTANTIAL',
    HIGH = 'HIGH',
}

export enum ReasonNamesByAction {
    STOP_EID = 'STOP_REASON_TYPE',
    RESUME_EID = 'RESUME_REASON_TYPE',
    REVOKE_EID = 'REVOKE_REASON_TYPE',
    ISSUE_EID = 'ISSUE_REASON_TYPE',
    DENY_APPLICATION = 'DENIED_REASON_TYPE',
}

export enum ApplicationType {
    ISSUE_EID = 'ISSUE_EID',
    RESUME_EID = 'RESUME_EID',
    REVOKE_EID = 'REVOKE_EID',
    STOP_EID = 'STOP_EID',
}

export enum NomenclatureNameWithAdditionalReasonField {
    STOPPED_OTHER = 'STOPPED_OTHER',
    RESUMED_OTHER = 'RESUMED_OTHER',
    DENIED_OTHER = 'DENIED_OTHER',
    REVOKED_OTHER = 'REVOKED_OTHER',
    REVOKED_BY_ADMINISTRATOR = 'REVOKED_BY_ADMINISTRATOR',
    STOPPED_BY_ADMINISTRATOR = 'STOPPED_BY_ADMINISTRATOR',
    DENIED_BY_ADMINISTRATOR = 'DENIED_BY_ADMINISTRATOR',
}

export enum SubmissionType {
    DESK = 'DESK',
    BASE_PROFILE = 'BASE_PROFILE',
    EID = 'EID',
    PERSO_CENTRE = 'PERSO_CENTRE',
}

export enum SCErrorType {
    NoError = 'NoError',
    BadInputParameters = 'BadInputParameters',
    SomethingWentWrong = 'SomethingWentWrong',
    PINIsBlocked = 'PINIsBlocked',
    CANIsBlocked = 'CANIsBlocked',
    PUKIsBlocked = 'PUKIsBlocked',
    PINIsNotValid = 'PINIsNotValid',
    CANIsNotValid = 'CANIsNotValid',
    PUKIsNotValid = 'PUKIsNotValid',
    CreatingCAError = 'CreatingCAError',
    CertificateValidationFailed = 'CertificateValidationFailed',
    DG14VerificationFailed = 'DG14VerificationFailed',
    DG14VerificationCrashed = 'DG14VerificationCrashed',
    MalformedEFSODSSCD = 'MalformedEFSODSSCD',
    FailedSelectingDG14 = 'FailedSelectingDG14',
    FailedReadingDG14 = 'FailedReadingDG14',
    SODHashAndSignatureVerificationFailed = 'SODHashAndSignatureVerificationFailed',
    CAGeneralAuthenticateFailed = 'CAGeneralAuthenticateFailed',
    CAManageSecurityEnvironmentFailed = 'CAManageSecurityEnvironmentFailed',
    OperationWasCanceled = 'OperationWasCanceled',
    DerivingPUKError = 'DerivingPUKError',
    PostCSRtoCAError = 'PostCSRtoCAError',
    ParseBase64CertificateError = 'ParseBase64CertificateError',
    CheckCardIsGenuineFailed = 'CheckCardIsGenuineFailed',
    MalformedPKParameter = 'MalformedPKParameter',
    CIAInfoIsNotInitializedProperly = 'CIAInfoIsNotInitializedProperly',
    CSRPkcs10Creation = 'CSRPkcs10Creation',
    PINIsNotActivated = 'PINIsNotActivated',
    PACECheckPinStatus = 'PACECheckPinStatus',
    PACEGetCardAccess = 'PACEGetCardAccess',
    PACEReadCardAccess = 'PACEReadCardAccess',
    PACEIncorectCardAccess = 'PACEIncorectCardAccess',
    PACESelectApplication = 'PACESelectApplication',
    PACEManageSecurityEnvironment = 'PACEManageSecurityEnvironment',
    PACERequestNonce = 'PACERequestNonce',
    PACEMappingData = 'PACEMappingData',
    PACEEphimeralKey = 'PACEEphimeralKey',
    PACESendToken = 'PACESendToken',
    DuringActivatePIN = 'DuringActivatePIN',
    CitizenCertificateCannotBeSelected = 'CitizenCertificateCannotBeSelected',
    CitizenCertificateCannotBeRead = 'CitizenCertificateCannotBeRead',
    CIACannotBeSelected = 'CIACannotBeSelected',
    SignManageSecurityEnvironmentFailed = 'SignManageSecurityEnvironmentFailed',
    SignSecurityOperationFailded = 'SignSecurityOperationFailded',
    ChangePINFailed = 'ChangePINFailed',
    PuK2ReadFailed = 'PuK2ReadFailed',
    GenerateKeyPair3Failed = 'GenerateKeyPair3Failed',
    CertificateCannotBeSelected = 'CertificateCannotBeSelected',
    WriteCertificateFailed = 'WriteCertificateFailed',
    PuK2SelectFailed = 'PuK2SelectFailed',
    PuK2UpdateFailed = 'PuK2UpdateFailed',
    CIAReadFailed = 'CIAReadFailed',
    GenerateKeyPair2Failed = 'GenerateKeyPair2Failed',
}

export enum PermittedUser {
    PRIVATE = 'PRIVATE',
    ADMIN = 'ADMIN',
    PUBLIC = 'PUBLIC',
}
