export enum SignatureProvider {
    NotSpecified = 'NotSpecified',
    KEP = 'KEP',
    Evrotrust = 'Evrotrust',
    Borica = 'Borica',
}

export enum SignatureStatus {
    None = 0,
    Pending = 1,
    Signed = 2,
    Rejected = 3,
    Expired = 4,
    Failed = 5,
    Withdrawn = 6,
    Undeliverable = 7,
    OnHold = 99,
}

export enum BoricaSignatureStatus {
    IN_PROGRESS = 'IN_PROGRESS',
    COMPLETED = 'COMPLETED',
    REJECTED = 'REJECTED',
    SIGNED = 'SIGNED',
}

export enum BoricaSignatureTypeEnum {
    CADES_BASELINE_B_DETACHED = 'CADES_BASELINE_B_DETACHED',
    PADES_BASELINE_B = 'PADES_BASELINE_B',
    XADES_BASELINE_B_ENVELOPED = 'XADES_BASELINE_B_ENVELOPED',
}
