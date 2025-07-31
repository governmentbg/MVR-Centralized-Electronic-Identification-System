import { EmpowermentsDenialReason, EmpowermentStatementStatus } from '../enums/authorization-register.enum';

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
}

export interface IEmpoweredUid {
    uid?: string;
    uidType?: string;
}

export interface IVolumeOfRepresentation {
    code: string;
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
    onBehalfOf: string;
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
}

export interface IEmpowermentXMLRepresentation {
    id: string;
    uid: string;
    onBehalfOf: string;
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
    providerId?: string;
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
