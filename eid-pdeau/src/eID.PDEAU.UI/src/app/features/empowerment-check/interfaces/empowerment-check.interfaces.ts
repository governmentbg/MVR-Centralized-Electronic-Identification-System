import { EmpowermentStatementStatus, OnBehalfOf } from '@app/features/empowerment-check/enums/empowerment-check.enum';

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
    id: string;
    name: string;
}

export interface IEmpowerment {
    id: string;
    startDate: string;
    expiryDate: string | null;
    status: string;
    uid: string;
    uidType: string;
    name: string;
    onBehalfOf: string;
    authorizerUids: IAuthorizerUid[];
    empoweredUids: IAuthorizerUid[];
    providerId: string;
    providerName: string;
    serviceId: number;
    serviceName: string;
    volumeOfRepresentation: IVolumeOfRepresentation[];
    createdOn: string;
    createdBy: string;
    issuerPosition: string;
    xmlRepresentation: string;
    denialReason: string;
    empowermentWithdrawals: IEmpowermentWithdrawal[];
    empowermentDisagreements: IEmpowermentDisagreement[];
    statusHistory: IEmpowermentStatusHistoryItem[];
    calculatedStatusOn: string;
    number: string;
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

export interface IEmpowermentDenyRequestData {
    empowermentId: string;
    denialReasonComment: string;
}

export interface IEmpowermentApproveRequestData {
    empowermentId: string;
}

export interface IEmpowermentCheckRequestData {
    onBehalfOf?: OnBehalfOf;
    volumeOfRepresentation?: string[];
    statusOn?: Date;
    serviceId: number;
    empoweredUid: string;
    authorizerUid: string;
    pageIndex: number;
    pageSize: number;
}

interface IActualStateResponseV3 {
    actualStateResponseV3: {
        deed: {
            companyName: string;
            uic: string;
            legalForm: string;
            caseNo: number;
            caseYear: number;
            subdeeds: {
                subdeed: {
                    records: {
                        record: {
                            recordData: {
                                wayOfRepresentation: {
                                    jointly: boolean;
                                    value: string;
                                };
                                representative: {
                                    subject: {
                                        indent: string;
                                        name: string;
                                        indentType: string;
                                        countryID: string;
                                        countryName: string;
                                    };
                                    person: any;
                                };
                            };
                            recordId: string;
                            incomingId: string;
                            fieldIdent: string;
                            mainField: {
                                groupId: number;
                                groupName: string;
                                providerSectionId: number;
                                sectionName: string;
                                mainFieldIdent: string;
                            };
                            fieldActionDate: string;
                        }[];
                    };
                }[];
            };
        };
        dataValidForDate: string;
        dataFound: string;
    };
}

interface IStateOfPlayResponseType {
    StateOfPlayResponseType: {
        returnInformations: {
            returnCode: string;
        };
        subject?: {
            uic: {
                uiC1: string;
                uicType: {
                    code: string;
                };
            };
            subjectType: {
                code: string;
            };
            legalEntitySubject: {
                country: {
                    code: string;
                };
                legalForm: {
                    code: string;
                };
                legalStatute: {
                    code: string;
                };
                subjectGroup: {
                    code: string;
                };
                cyrillicFullName: string;
                cyrillicShortName: string;
                latinFullName: string;
                subordinateLevel: {
                    code: string;
                };
                trStatus: string;
            };
        };
        event?: {
            eventType: {
                code: string;
            };
            entryType: {
                code: string;
            };
        };
        state?: {
            state: {
                code: string;
            };
        };
        managers?: {
            relatedSubject: {
                naturalPersonSubject?: {
                    country: {
                        code: string;
                    };
                    egn: string;
                    lnc: string;
                    cyrillicName: string;
                    latinName: string;
                };
            };
            position: {
                code: string;
            };
            representedSubjects: any[];
        }[];
    };
}

type RegixOrBulstatResponse = IActualStateResponseV3 | IStateOfPlayResponseType;
export interface ILegalEntityInfoOriginalResponse {
    isFoundInRegix: boolean;
    originalResponse: {
        error: string | null;
        hasFailed: boolean;
        response: RegixOrBulstatResponse;
    };
}

export interface ILegalEntityInfoResponse {
    legalEntityName: string;
    uic: string;
    wayOfRepresentation: string;
    legalRepresentatives: {
        name: string;
        uid: string;
        uidType: string;
    }[];
    state: string;
}

export interface IReason {
    id: string;
    translations: { language: string; name: string }[];
}
