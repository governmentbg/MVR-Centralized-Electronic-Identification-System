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

export enum ApplicationTypes {
    ISSUE_EID = 'ISSUE_EID',
    RESUME_EID = 'RESUME_EID',
    REVOKE_EID = 'REVOKE_EID',
    STOP_EID = 'STOP_EID',
}

export enum SubmissionTypes {
    EID = 'EID',
    DESK = 'DESK',
    BASE_PROFILE = 'BASE_PROFILE',
}

export enum CertificateStatus {
    CREATED = 'CREATED',
    ACTIVE = 'ACTIVE',
    STOPPED = 'STOPPED',
    REVOKED = 'REVOKED',
    EXPIRED = 'EXPIRED',
}

export enum SortOrder {
    'desc' = 1,
    'asc' = -1,
}

export enum ReasonNamesByAction {
    STOP_EID = 'STOP_REASON_TYPE',
    RESUME_EID = 'RESUME_REASON_TYPE',
    REVOKE_EID = 'REVOKE_REASON_TYPE',
    ISSUE_EID = 'ISSUE_REASON_TYPE',
    DENY_APPLICATION = 'DENIED_REASON_TYPE',
}

export enum ReasonNamesByStatus {
    STOPPED = 'STOP_REASON_TYPE',
    RESUMED = 'RESUME_REASON_TYPE',
    REVOKED = 'REVOKE_REASON_TYPE',
    ISSUED = 'ISSUE_REASON_TYPE',
    DENIED = 'DENIED_REASON_TYPE',
}

export enum PermittedUser {
    PRIVATE = 'PRIVATE',
    ADMIN = 'ADMIN',
    PUBLIC = 'PUBLIC',
}

export enum OfficeType {
    ONLINE = 'ONLINE',
}

export enum ILevelOfAssurances {
    HIGH = 'HIGH',
    SUBSTANTIAL = 'SUBSTANTIAL',
}
