export enum ApplicationStatus {
    IN_REVIEW = 'IN_REVIEW',
    ACTIVE = 'ACTIVE',
    DENIED = 'DENIED',
    RETURNED_FOR_CORRECTION = 'RETURNED_FOR_CORRECTION',
    ACCEPTED = 'ACCEPTED',
}

export enum ApplicationType {
    REGISTER = 'REGISTER',
    RESUME = 'RESUME',
    REVOKE = 'REVOKE',
    STOP = 'STOP',
}

export enum eidManagerStatus {
    ACTIVE = 'ACTIVE',
    SUSPENDED = 'SUSPENDED',
    STOPPED = 'STOPPED',
    IN_REVIEW = 'IN_REVIEW',
    PENDING_ATTACHMENTS = 'PENDING_ATTACHMENTS',
    TECHNICAL_CHECK = 'TECHNICAL_CHECK',
}

export enum CircumstanceStatus {
    APPROVED = 'APPROVED',
    FOR_CORRECTION = 'FOR_CORRECTION',
    SUSPENDED = 'SUSPENDED',
    STOPPED = 'STOPPED',
    ACTIVE = 'ACTIVE',
}

export enum DeviceType {
    CHIP_CARD = 'CHIP_CARD',
    MOBILE = 'MOBILE',
    OTHER = 'OTHER',
}
