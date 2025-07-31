import {
    ApplicationStatus,
    ApplicationType,
    CertificateStatus,
    DeviceType,
    LevelOfAssurance,
    PermittedUser,
    PersonalIdTypes,
    SubmissionType,
} from '../enums/eid-management.enum';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';

export interface IFindEidentityByNumberAndTypeRequestData {
    number: string;
    type: PersonalIdTypes;
    email?: string | null;
}

export interface IFindEidentityByNumberAndTypeResponseData {
    eidentityId: string | null;
    active: boolean | null;
    firstName: string | null;
    secondName: string | null;
    lastName: string | null;
    citizenIdentifierNumber: string | null;
    citizenIdentifierType: string | null;
    citizenProfileId: string | null;
    email: string | null;
    phoneNumber: string | null;
}

export interface IGetApplicationsByEidentityRequestData {
    eidentityId: string;
    page: number;
    size: number;
    sort: string;
    id?: number;
    status?: ApplicationStatus;
    createDate?: string;
    deviceId?: string;
}

export interface IGetCertificatesByEidentityRequestData {
    eidentityId: string;
    page: number;
    size: number;
    sort: string;
    serialNumber?: number;
    status?: CertificateStatus;
    validityFrom?: string;
    validityUntil?: string;
    deviceId?: string;
}

export interface IGetApplicationsByEidentityResponseData {
    content: IApplication[];
    pageable: {
        sort: {
            empty: boolean;
            sorted: boolean;
            unsorted: boolean;
        };
        offset: number;
        pageNumber: number;
        pageSize: number;
        paged: boolean;
        unpaged: boolean;
    };
    last: boolean;
    totalElements: number;
    totalPages: number;
    size: number;
    number: number;
    sort: {
        empty: boolean;
        sorted: boolean;
        unsorted: boolean;
    };
    first: boolean;
    numberOfElements: number;
    empty: boolean;
}

export interface IGetCertificatesByEidentityResponseData {
    content: ICertificate[];
    pageable: {
        sort: {
            empty: boolean;
            sorted: boolean;
            unsorted: boolean;
        };
        offset: number;
        pageNumber: number;
        pageSize: number;
        paged: boolean;
        unpaged: boolean;
    };
    last: boolean;
    totalElements: number;
    totalPages: number;
    size: number;
    number: number;
    sort: {
        empty: boolean;
        sorted: boolean;
        unsorted: boolean;
    };
    first: boolean;
    numberOfElements: number;
    empty: boolean;
}

export interface IApplication {
    status: ApplicationStatus;
    createDate: string;
    deviceId: string;
    eidentityId: string;
    id: string;
    certificateId?: string;
    serialNumber?: string;
    phoneNumber?: string;
    email?: string;
    eidAdministratorName: string;
    reasonId: string | null;
    reasonText: string | null;
    applicationType: ApplicationType;
    submissionType: SubmissionType;
    applicationNumber: string;
    firstName: string;
    secondName: string;
    lastName: string;
    firstNameLatin: string;
    secondNameLatin: string;
    lastNameLatin: string;
    citizenship: string;
    eidAdministratorOfficeName: string;
    identityNumber: string;
    identityType: PersonalIdTypes;
    identityIssueDate: string;
    identityValidityToDate: string;
    fee: number | null;
    feeCurrency: string | null;
    secondaryFee: number | null;
    secondaryFeeCurrency: string | null;
    eidAdministratorFrontOfficeId: string;
    numForm: string | null;
    operatorUsername: string | null;
}

export interface ICertificate {
    id: string;
    status: CertificateStatus;
    eidAdministratorId: string;
    eidentityId: string;
    commonName: string;
    validityFrom: string;
    validityUntil: string;
    createDate: string;
    serialNumber: string;
    deviceId: string;
    levelOfAssurance: LevelOfAssurance;
    eidAdministratorName: string;
    isExpiring: boolean;
    alias: string;
}

export interface ICreateEidApplicationRequestData {
    eidentityId: string;
    firstName: string;
    secondName: string;
    lastName: string;
    firstNameLatin: string;
    secondNameLatin: string;
    lastNameLatin: string;
    citizenIdentifierNumber: string;
    citizenIdentifierType: PersonalIdTypes;
    applicationSubmissionType: string;
    applicationType: string;
    deviceId: string;
    signedDetails?: string;
    levelOfAssurance: LevelOfAssurance;
    reasonId?: string;
    reasonText?: string;
    certificateId?: string;
    citizenship: string;
    personalIdentityDocument: {
        identityNumber: string;
        identityType: string;
        identityIssueDate: string;
        identityValidityToDate: string;
        identityIssuer: string;
    };
}

export interface ICreateEidApplicationResponseData {
    id: string;
    applicationNumber: string;
    status: ApplicationStatus;
    eidAdministratorName: string;
    fee: number | null;
    feeCurrency: string | null;
    secondaryFee: number | null;
    secondaryFeeCurrency: string | null;
}

export interface IUpdateEidApplicationRequestData {
    applicationId: string;
    status: ApplicationStatus;
}

export interface IExportEidApplicationRequestData {
    applicationId: string;
}

export interface IGetPersonalIdentityV2ResponseData {
    response: {
        PersonalIdentityInfoResponse: IPersonalIdentity;
    };
    hasFailed: boolean;
    error: any;
}

export interface IGetForeignIdentityV2ResponseData {
    response: {
        ForeignIdentityInfoResponse: IForeignIdentity;
    };
    hasFailed: boolean;
    error: any;
}

export interface IRelationsSearchResponseData {
    response: {
        relationsResponse: {
            personRelations?: IPersonRelation[];
        };
    };
    hasFailed: boolean;
    error: any;
}

export interface IForeignIdentity {
    returnInformations: {
        returnCode?: string;
        info?: string;
    };
    egn: string;
    personNames?: {
        firstName?: string;
        surname?: string;
        familyName?: string;
        firstNameLatin?: string;
        surnameLatin?: string;
        lastNameLatin?: string;
    };
    dLCategories?: {
        dLCategory?: {
            category?: string;
            dateCategory?: string;
            endDateCategory?: string;
            restrictions?: string[];
        }[];
    };
    dLCommonRestrictions?: string;
    dataForeignCitizen?: {
        pin?: string;
        pn?: string;
        names?: {
            firstName?: string;
            surname?: string;
            familyName?: string;
            firstNameLatin?: string;
            surnameLatin?: string;
            lastNameLatin?: string;
        };
        nationalityList: {
            nationalityCode2?: string;
            nationalityName2?: string;
            nationalityCode?: string;
            nationalityName?: string;
            nationalityNameLatin?: string;
        }[];
        gender?: {
            genderCode?: string;
            cyrillic?: string;
            latin?: string;
        };
        birthDate?: string;
    };
    rPRemarks?: {
        rPRemark?: string[];
    };
    rPTypeofPermit?: string;
    deathDate?: string;
    birthDate?: string;
    birthDateSpecified?: boolean;
    birthPlace?: {
        countryCode?: string;
        countryName?: string;
        countryNameLatin?: string;
        territorialUnitName?: string;
        districtName?: string;
        municipalityName?: string;
    };
    genderName?: string;
    genderNameLatin?: string;
    nationalityList: {
        nationalityCode2?: string;
        nationalityName2?: string;
        nationalityCode?: string;
        nationalityName?: string;
        nationalityNameLatin?: string;
    }[];
    permanentAddress?: {
        districtName?: string;
        districtNameLatin?: string;
        municipalityName?: string;
        municipalityNameLatin?: string;
        settlementCode?: string;
        settlementName?: string;
        settlementNameLatin?: string;
        locationCode?: string;
        locationName?: string;
        locationNameLatin?: string;
        buildingNumber?: string;
        entrance?: string;
        floor?: string;
        apartment?: string;
    };
    documentType?: string;
    documentTypeLatin?: string;
    identityDocumentNumber?: string;
    issueDate?: string;
    issueDateSpecified?: boolean;
    issuePlace?: string;
    issuePlaceLatin?: string;
    issuerName?: string;
    issuerNameLatin?: string;
    rpTypeOfPermit?: string;
    statusDate?: string;
    statusCyrillic?: string;
    validDate?: string;
    validDateSpecified?: boolean;
    identityDocument?: {
        documentType?: string;
        documentTypeLatin?: string;
        identityDocumentNumber?: string;
        issueDate?: string;
        issueDateSpecified?: boolean;
        issuePlace?: string;
        issuePlaceLatin?: string;
        issuerName?: string;
        issuerNameLatin?: string;
        rpTypeOfPermit?: string;
        statusDate?: string;
        statusCyrillic?: string;
        validDate?: string;
        validDateSpecified?: boolean;
    };
    travelDocument?: {
        documentType?: string;
        documentTypeLatin?: string;
        identityDocumentNumber?: string;
        issueDate?: string;
        issueDateSpecified?: boolean;
        issuePlace?: string;
        issuePlaceLatin?: string;
        issuerName?: string;
        issuerNameLatin?: string;
        rpTypeOfPermit?: string;
        statusDate?: string;
        statusCyrillic?: string;
        validDate?: string;
        validDateSpecified?: boolean;
    };
    height?: number;
    heightSpecified?: boolean;
    eyesColor?: string;
    picture?: string;
    identitySignature?: string;
}

export interface IPersonalIdentity {
    returnInformations: {
        returnCode?: string;
        info?: string;
    };
    egn?: string;
    personNames: {
        firstName?: string;
        surname?: string;
        familyName?: string;
        firstNameLatin?: string;
        surnameLatin?: string;
        lastNameLatin?: string;
    };
    dLCategories?: {
        dLCategory?: {
            category?: string;
            dateCategory?: string;
            endDateCategory?: string;
            restrictions?: string[];
        }[];
    };
    dLCommonRestrictions?: string;
    dataForeignCitizen?: {
        pin?: string;
        pn?: string;
        names?: {
            firstName?: string;
            surname?: string;
            familyName?: string;
            firstNameLatin?: string;
            surnameLatin?: string;
            lastNameLatin?: string;
        };
        nationalityList: {
            nationalityCode2?: string;
            nationalityName2?: string;
            nationalityCode?: string;
            nationalityName?: string;
            nationalityNameLatin?: string;
        }[];
        gender?: {
            genderCode?: string;
            cyrillic?: string;
            latin?: string;
        };
        birthDate?: string;
        birthDateSpecified?: boolean;
    };
    rPRemarks?: {
        rPRemark?: string[];
    };
    rPTypeofPermit?: string;
    documentType?: string;
    documentTypeLatin?: string;
    documentActualStatus?: string;
    actualStatusDate?: string;
    actualStatusDateSpecified?: boolean;
    documentStatusReason?: string;
    identityDocumentNumber?: string;
    issueDate?: string;
    issueDateSpecified?: boolean;
    issuerPlace?: string;
    issuerPlaceLatin?: string;
    issuerName?: string;
    issuerNameLatin?: string;
    validDate?: string;
    validDateSpecified?: boolean;
    birthDate?: string;
    birthDateSpecified?: boolean;
    birthPlace?: {
        countryCode?: string;
        countryName?: string;
        countryNameLatin?: string;
        territorialUnitName?: string;
        districtName?: string;
        municipalityName?: string;
    };
    genderName?: string;
    genderNameLatin?: string;
    nationalityList: {
        nationalityCode2?: string;
        nationalityName2?: string;
        nationalityCode?: string;
        nationalityName?: string;
        nationalityNameLatin?: string;
    }[];
    permanentAddress?: {
        districtName?: string;
        districtNameLatin?: string;
        municipalityName?: string;
        municipalityNameLatin?: string;
        settlementCode?: string;
        settlementName?: string;
        settlementNameLatin?: string;
        locationCode?: string;
        locationName?: string;
        locationNameLatin?: string;
        buildingNumber?: string;
        entrance?: string;
        floor?: string;
        apartment?: string;
    };
    height?: number;
    heightSpecified?: boolean;
    eyesColor?: string;
    picture?: string;
    identitySignature?: string;
}

export interface IPersonRelation {
    relationCode: string;
    egn: string;
    birthDate: string;
    firstName: string;
    surName: string;
    familyName: string;
    gender: {
        genderCode: string;
        genderName: string;
    };
    nationality: {
        nationalityCode: string;
        nationalityName: string;
        nationalityCode2?: string;
        nationalityName2?: string;
    };
    deathDate: string;
}

export interface IHidePreviewEventEmitter {
    showPreview: boolean;
    refreshTable?: boolean;
    requestedAction?: {
        name: ApplicationType;
        context?: any;
    };
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
    permittedUser: PermittedUser;
}

export interface IHistoryItem {
    status: CertificateStatus;
    applicationId: string;
    modifiedDateTime: string;
    reasonId: string;
    reasonText: string;
    createdDateTime: string;
    validityFrom: string;
    validityUntil: string;
    applicationNumber: string;
}

export interface ISignApplicationRequestData {
    id: string;
    email?: string;
    phoneNumber?: string;
    guardians?: IGuardian[];
}

export interface IGuardian {
    firstName: string;
    secondName: string;
    lastName: string;
    citizenIdentifierNumber: string;
    citizenIdentifierType: string;
    citizenship: string;
    personalIdentityDocument: IPersonalIdentityDocument;
}

export interface IPersonalIdentityDocument {
    identityNumber: string;
    identityType: string;
    identityIssueDate: string;
    identityValidityToDate: string;
    identityIssuer: string;
}

type GuardianKeys = {
    [key in keyof IGuardian]: AbstractControl;
};

export interface IGuardingKeysForm extends GuardianKeys {
    personalIdentityDocument: FormGroup;
}

export interface ISaveCertificateRequestData {
    readerName: string;
    token: string;
    can: string;
    applicationId: string;
    certificateAuthorityName: string;
    countryCode: string;
    commonName: string;
    givenName: string;
    surname: string;
    serialNumber: string;
    puk?: string;
    cardNumber: string;
}

export interface IGetCertificateInformationRequestData {
    readerName: string;
    can: string;
}

export interface IPINRequestData {
    readerName: string;
    can: string;
    puk?: string;
    token: string;
}

export interface IGetCertificateInformationResponseData {
    isGenuine: boolean;
    isPinActivated: boolean;
    certificateInfo: string;
    cardNumber: string;
}

export interface IStep {
    id: number;
    name: string;
    currentInnerStep?: number;
    innerSteps?: { id: number; name: string }[];
    ref: string;
}

export interface IDevice {
    id: string;
    name: string;
    type: DeviceType;
    description: string;
}

export interface IDeviceWithTranslations {
    id: string;
    type: DeviceType;
    translations: {
        language: string;
        name: string;
    }[];
}

export interface ICardCommunicationForm {
    readerName: FormControl<string>;
    can: FormControl<string>;
    puk?: FormControl<string>;
}

export const LAWFUL_AGE = 18;
export const CHILD_AGE = 14;

export interface IAddGuardiansRequestData {
    guardians?: IGuardian[];
}

export interface IAdministrator {
    id: string;
    name: string;
    nameLatin: string;
    eikNumber: string;
    isActive: boolean;
    contact: string;
    homePage: string;
    logo: string;
    address: string;
    eidManagerFrontOfficeIds?: string;
    eidFrontOffices: IAdministratorOffice[];
    deviceIds: string[];
}

export interface IAdministratorOffice {
    id: string;
    name: string;
    region: string;
}

export interface ICarrier {
    id: string;
    serialNumber: string;
    type: string;
    certificateId: string;
    eId: string;
    modifiedOn: string;
}

export interface ICheckUidRestrictionsResponseData {
    response: {
        isProhibited: boolean;
        isDead: boolean;
        hasRevokedParentalRights: boolean;
    };
    hasFailed: boolean;
    error: string;
}
