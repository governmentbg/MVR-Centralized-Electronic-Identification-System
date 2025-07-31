import { z } from 'zod';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import {
    ApplicationStatus,
    ApplicationType,
    eidManagerStatus,
} from '@app/features/administrators/enums/administrators.enum';

export const userSchema = z.object({
    id: z.string(),
    createdOn: z.string(),
    uidType: z.string(),
    uid: z.string(),
    name: z.string(),
    email: z.string(),
    isAdministrator: z.boolean(),
    phone: z.string(),
});

export const fileSchema = z.object({
    id: z.string(),
    uploaderUid: z.string(),
    uploaderUidType: z.string(),
    uploaderName: z.string(),
    fileName: z.string(),
    filePath: z.string(),
    uploadedOn: z.string(),
});

export type FileType = z.infer<typeof fileSchema>;

export const SortSchema = z.object({
    empty: z.boolean(),
    sorted: z.boolean(),
    unsorted: z.boolean(),
});

export const PageableSchema = z.object({
    sort: z.any(),
    offset: z.number(),
    pageNumber: z.number(),
    pageSize: z.number(),
    paged: z.boolean(),
    unpaged: z.boolean(),
});

export const AdministratorApplicationBasicSchema = z.object({
    applicationNumber: z.string(),
    applicationType: z.nativeEnum(ApplicationType),
    eidName: z.string(),
    status: z.nativeEnum(ApplicationStatus),
    id: z.string(),
    createDate: z.string(),
});

export const AdministratorsPageableSchema = z.object({
    content: z.array(AdministratorApplicationBasicSchema),
    pageable: PageableSchema,
    last: z.boolean(),
    totalElements: z.number(),
    totalPages: z.number(),
    size: z.number(),
    number: z.number(),
    sort: z.any(),
    first: z.boolean(),
    numberOfElements: z.number(),
    empty: z.boolean(),
});

export const descriptionsSchema = z.object({
    EN: z.string(),
    BG: z.string(),
});

export const documentTypeSchema = z.object({
    id: z.string().uuid(),
    name: z.string(),
    active: z.boolean(),
    requiredForAdministrator: z.boolean().nullish(),
    requiredForCenter: z.boolean().nullish(),
    descriptions: z.record(z.string(), z.any()),
});

export const documentTypeListSchema = z.array(documentTypeSchema);

export type documentType = z.infer<typeof documentTypeSchema>;

export const EmployeeSchema = z.object({
    roles: z.array(z.string()),
    name: z.string(),
    nameLatin: z.string(),
    phoneNumber: z.string(),
    email: z.string(),
    citizenIdentifierType: z.string(),
    citizenIdentifierNumber: z.string(),
});

export const OperatorSchema = z.object({
    name: z.string(),
    nameLatin: z.string(),
    phoneNumber: z.string(),
    email: z.string(),
    citizenIdentifierType: z.string(),
    citizenIdentifierNumber: z.string(),
});

export const EidManagerFrontOfficeSchema = z.object({
    name: z.string(),
    location: z.string(),
    region: z.string(),
    contact: z.string(),
    longitude: z.string(),
    latitude: z.string(),
    workingHours: z.array(z.string()),
    email: z.string(),
    description: z.string(),
});

export const DeviceSchema = z.object({
    name: z.string(),
    type: z.string(),
    description: z.string(),
    authorizationLink: z.string().nullish(),
    backchannelAuthorizationLink: z.string().nullish(),
});

export const PersonSchema = z.object({
    name: z.string(),
    nameLatin: z.string(),
    phoneNumber: z.string().nullish(),
    email: z.string(),
    citizenIdentifierType: z.string(),
    citizenIdentifierNumber: z.string(),
});

export const AttachmentSchema = z.object({
    fileName: z.string(),
    content: z.string(),
    documentType: z.string(),
});

export type Attachment = z.infer<typeof AttachmentSchema>;

export const AttachmentFullSchema = z.object({
    id: z.string().uuid(),
    fileName: z.string(),
    content: z.string(),
    documentType: documentTypeSchema,
    isOutgoing: z.boolean().nullish(),
    createDate: z.string(),
});

export type AttachmentFull = z.infer<typeof AttachmentFullSchema>;

export const ApplicationSchema = z.object({
    referenceNumber: z.string(),
    employees: z.array(EmployeeSchema),
    eidManagerFrontOffices: z.array(EidManagerFrontOfficeSchema),
    devices: z.array(DeviceSchema),
    authorizedPersons: z.array(PersonSchema),
    applicant: PersonSchema,
    applicationType: z.string(),
    attachments: z.array(AttachmentSchema),
    homePage: z.string(),
    companyName: z.string(),
    companyNameLatin: z.string(),
    eikNumber: z.string(),
    address: z.string(),
    email: z.string(),
    phone: z.string(),
    description: z.string(),
    logoUrl: z.string().optional(),
    downloadUrl: z.string().optional(),
    applicationNumber: z
        .object({
            id: z.string(),
            applicationType: z.string(),
        })
        .optional(),
});

export type AdministratorApplication = z.infer<typeof ApplicationSchema>;

export const ApplicationDetailsSchema = z.object({
    id: z.string(),
    referenceNumber: z.string().nullish(),
    employees: z.array(EmployeeSchema),
    eidManagerFrontOffices: z.array(EidManagerFrontOfficeSchema),
    devices: z.array(DeviceSchema),
    authorizedPersons: z.array(PersonSchema),
    applicant: PersonSchema,
    applicationType: z.string(),
    attachments: z.array(AttachmentFullSchema),
    homePage: z.string().nullish(),
    companyName: z.string(),
    companyNameLatin: z.string(),
    eikNumber: z.string(),
    address: z.string(),
    email: z.string(),
    phone: z.string().nullish(),
    description: z.string().nullish(),
    status: z.nativeEnum(ApplicationStatus),
    notes: z.array(z.any()),
    applicationNumber: z.object({
        id: z.string(),
        applicationType: z.string(),
    }),
});

export type ApplicationDetails = z.infer<typeof ApplicationDetailsSchema>;

export const AdministratorDetailsSchema = z.object({
    id: z.string(),
    referenceNumber: z.string().nullish(),
    employees: z.array(EmployeeSchema),
    eidManagerFrontOffices: z.array(EidManagerFrontOfficeSchema),
    devices: z.array(DeviceSchema),
    authorizedPersons: z.array(PersonSchema),
    applicant: PersonSchema,
    applicationType: z.string(),
    attachments: z.array(AttachmentFullSchema),
    homePage: z.string(),
    name: z.string(),
    nameLatin: z.string(),
    eikNumber: z.string(),
    address: z.string(),
    email: z.string(),
    phone: z.string(),
    description: z.string(),
    managerStatus: z.nativeEnum(eidManagerStatus),
    notes: z.array(z.any()),
});

export type AdministratorDetails = z.infer<typeof AdministratorDetailsSchema>;

export interface EmployeeForm {
    roles: FormControl<string[]>;
    name: FormControl<string>;
    nameLatin: FormControl<string>;
    phoneNumber: FormControl<string>;
    email: FormControl<string>;
    citizenIdentifierType: FormControl<string>;
    citizenIdentifierNumber: FormControl<string>;
}

export interface EidManagerFrontOfficeForm {
    name: FormControl<string>;
    location: FormControl<string>;
    region: FormControl<string>;
    contact: FormControl<string>;
    longitude: FormControl<string>;
    latitude: FormControl<string>;
    workingHours: FormControl<string[]>;
    email: FormControl<string>;
    description: FormControl<string>;
}

export interface DeviceForm {
    name: FormControl<string>;
    type: FormControl<string>;
    description: FormControl<string>;
    authorizationLink: FormControl<string | null>;
    backchannelAuthorizationLink: FormControl<string | null>;
}

export interface PersonForm {
    name: FormControl<string>;
    nameLatin: FormControl<string>;
    phoneNumber: FormControl<string>;
    email: FormControl<string>;
    citizenIdentifierType: FormControl<string>;
    citizenIdentifierNumber: FormControl<string>;
}

export interface AttachmentForm {
    fileName: FormControl<string>;
    content: FormControl<string>;
    documentType: FormControl<string>;
}

export interface ApplicationForm {
    referenceNumber: FormControl<string>;
    employees: FormArray<FormGroup<EmployeeForm>>;
    eidManagerFrontOffices: FormArray<FormGroup<EidManagerFrontOfficeForm>>;
    devices: FormArray<FormGroup<DeviceForm>>;
    authorizedPersons: FormArray<FormGroup<PersonForm>>;
    applicant: FormGroup<PersonForm>;
    applicationType: FormControl<string>;
    attachments: FormArray<FormGroup<AttachmentForm>>;
    homePage: FormControl<string>;
    companyName: FormControl<string>;
    companyNameLatin: FormControl<string>;
    eikNumber: FormControl<string>;
    address: FormControl<string>;
    email: FormControl<string>;
    phone: FormControl<string>;
    description: FormControl<string>;
    logoUrl: FormControl<string>;
    downloadUrl: FormControl<string>;
}

export interface documentTypeFormArray {
    files: FormArray<FormGroup<AttachmentForm>>;
    documentType: FormControl<string>;
}

export interface documentTypeFormGroup {
    attachments: FormArray<FormGroup<documentTypeFormArray>>;
}

export const RegionSchema = z.object({
    code: z.string(),
    descriptions: z.object({
        en: z.string(),
        bg: z.string(),
    }),
});

export type Region = z.infer<typeof RegionSchema>;

export const RegionListSchema = z.array(RegionSchema);
