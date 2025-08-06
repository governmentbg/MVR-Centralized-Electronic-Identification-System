import {
    ApplicationTypes,
    CertificateStatus,
    ILevelOfAssurances,
    PermittedUser,
    SubmissionTypes,
} from '../enums/eid-management';

export interface IStep {
    id: number;
    name: string;
    currentInnerStep?: number;
    innerSteps?: { id: number; name: string }[];
}

export interface IApplication {
    id: string;
    applicationType: string;
    certificateId: string;
    serialNumber: string;
    applicationNumber: string;
    status: string;
    createDate: Date;
    deviceId: string;
    eidAdministratorName: string;
    eidAdministratorOfficeName: string;
    eidentityId: string;
    email: string;
    identityNumber: string;
    identityIssueDate: string;
    identityValidityToDate: string;
    identityIssuer: string;
    phoneNumber: string;
    reasonId: string;
    reasonText: string;
    xml: string;
    submissionType: string;
    identityType: string;
    firstName: string;
    secondName: string;
    lastName: string;
    paymentAccessCode: string;
    paymentStatus: string;
}

export interface ICertificate {
    id: string;
    applicationNumber: string;
    status: string;
    eidAdministratorId: string;
    eidAdministratorOfficeId: string;
    eidAdministratorName: string;
    eidentityId: string;
    commonName: string;
    validityFrom: Date;
    validityUntil: Date;
    createDate: Date;
    serialNumber: string;
    alias: string;
    deviceId: string;
    levelOfAssurance: ILevelOfAssurances;
    reasonId: string;
    carrierSerialNumber: string;
    reasonText: string;
    isExpiring: boolean;
}

export interface IEidAdministrator {
    id: string;
    name: string;
    nameLatin: string;
    eikNumber: string;
    isActive: boolean;
    contact: string;
    logoUrl: string;
    homePage: string;
    address: string;
    administratorFrontOfficeIds: string[];
    deviceIds: string[];
}

export interface IDevices {
    id: string;
    name: string;
    type: string;
    description: string;
}

export interface IEidAdministratorOffice {
    id: string;
    name: string;
    eidAdministratorId: string;
    location: string;
    region: string;
    contact: string;
    isActive: boolean;
    latitude: number;
    longitude: number;
    email: string;
    workingHours: string[];
    devices: [
        {
            id: string;
            name: string;
            type: string;
            description: string;
        }
    ];
}
export interface Application {
    id: string;
    status: string;
    createDate: Date;
    eidentityId: string;
    eidAdministratorName: string;
    deviceId: string;
    applicationType: string;
    applicationNumber: string;
    serialNumber: string;
    reasonId: string;
    action: ApplicationTypes;
    submissionType: SubmissionTypes;
}

export interface IHideCertificatePreviewEventEmitter {
    showPreview: boolean;
    refreshTable: boolean;
    statusChangeRequest: boolean;
}

export interface IHideHelpPagePreviewEventEmitter {
    showPreview: boolean;
}

export interface IHistoryItem {
    status: CertificateStatus;
    applicationId: string;
    applicationNumber: string;
    modifiedDateTime: string;
    reasonId: string;
    reasonText: string;
    createdDateTime: string;
    validityFrom: string;
    validityUntil: string;
}

export interface IReason {
    id: string;
    name: string;
    nomenclatures: INomenclature[];
}

export interface INomenclature {
    id: string;
    name: string;
    description: string;
    language: string;
    textRequired: boolean;
    permittedUser: PermittedUser;
}

export interface Tariff {
    fee: number;
    feeCurrency: string;
    secondaryFee: number;
    secondaryFeeCurrency: string;
    paymentAccessCode: string;
    isPaymentRequired: boolean;
}

export interface Service {
    id: string;
    name: string;
    nameLatin: string;
    serviceType: string;
}
