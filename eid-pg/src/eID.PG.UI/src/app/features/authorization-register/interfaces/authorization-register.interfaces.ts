import { EmpowermentsDenialReason, EmpowermentStatementStatus, OnBehalfOf } from '../enums/authorization-register.enum';

export interface IEmpowermentPayload {
    sortby: string;
    sortDirection: number;
    pageIndex: number;
    pageSize: number;
    status?: string;
    serviceName?: string;
    providerName?: string;
    showOnlyNoExpiryDate?: string;
    validToDate?: string;
    authorizer?: string;
}

export interface IAuthorizerUid {
    uid?: string;
    uidType?: string;
    name?: string;
}

export interface IEmpoweredUid {
    uid?: string;
    uidType?: string;
    name?: string;
}

export interface IVolumeOfRepresentation {
    name: string;
}

export interface IEmpowerment {
    id: string;
    uid: string;
    status: string;
    authorizer: string;
    providerName: string;
    serviceName: string;
    authorizerUids: IAuthorizerUid[];
    empoweredUids: IEmpoweredUid[];
    expiryDate: string | null;
    modifiedBy: string;
    modifiedOn: string;
    name: string;
    onBehalfOf: OnBehalfOf | null;
    serviceId: number;
    startDate: string;
    providerId: string;
    volumeOfRepresentation: IVolumeOfRepresentation[];
    issuerPosition: string | null;
    empowermentWithdrawals: IEmpowermentWithdrawal[];
    xmlRepresentation: string;
    empowermentDisagreements?: IEmpowermentDisagreement[];
    denialReason: EmpowermentsDenialReason;
    createdOn: string;
    statusHistory: IEmpowermentStatusHistoryItem[];
    number: string;
}

export interface IEmpowermentWithSignature extends IEmpowerment {
    empowermentSignatures: EmpowermentSignature[];
}

export interface EmpowermentSignature {
    dateTime: string;
    signature: string;
    signerUid: string;
}

export interface IEmpowermentXMLRepresentation {
    id: string;
    uid: string;
    onBehalfOf: OnBehalfOf;
    name: string;
    authorizerUids: IAuthorizerUid[];
    empoweredUids: IEmpoweredUid[];
    providerId: string;
    providerName: string;
    serviceId: number;
    serviceName: string;
    volumeOfRepresentation: IVolumeOfRepresentation[];
    typeOfEmpowerment: string;
    createdOn: string;
    startDate: string;
    expiryDate: string;
    number: string;
}

export interface IEmpowermentStatusHistoryItem {
    id: string;
    status: EmpowermentStatementStatus;
    dateTime: string;
}

export interface IEmpowermentsPaginatedData {
    pageIndex: number;
    totalItems: number;
    data: IEmpowerment[];
}

export interface IEmpowermentRequestData {
    onBehalfOf: OnBehalfOf;
    issuerPosition: string | null;
    uid: string | null | undefined;
    uidType: string | null | undefined;
    name: string | null | undefined;
    empoweredUids: object[];
    typeOfEmpowerment: number;
    providerId: string;
    providerName: string;
    serviceId: number;
    serviceName: string;
    volumeOfRepresentation: { id: string; name: string }[];
    startDate: Date;
    expiryDate: Date | null;
    authorizerUids?: any[];
}

export interface IEmpowermentProvidersPayload {
    [key: string]: any;

    pageIndex?: number;
    pageSize?: number;
    name?: string;
    includeDeleted?: boolean;
}

export interface IEmpowermentServicesPayload {
    [key: string]: any;

    pageIndex?: number;
    pageSize?: number;
    serviceNumber?: number;
    name?: string;
    description?: string;
    findServiceNumberAndName?: string;
    includeEmpowermentOnly?: boolean;
    includeDeleted?: boolean;
    providerDetailsId?: string;
    providerSectionId?: string;
}

export interface IEmpowermentScopesPayload {
    [key: string]: any;

    serviceId?: string;
    includeDeleted?: boolean;
}

export interface IEmpowermentWithdrawal {
    activeDateTime: string | null;
    issuerUid: string;
    reason: string;
    startDateTime: string;
    status: string;
}

export interface IEmpowermentDisagreement {
    activeDateTime: string | null;
    issuerUid: string;
    reason: string;
    startDateTime: string;
    status: string;
}

export interface IHidePreviewEventEmitter {
    showPreview: boolean;
    refreshTable: boolean;
}
