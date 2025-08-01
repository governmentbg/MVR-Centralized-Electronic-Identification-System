import { z } from 'zod';

const issuerSchema = z.object({
    issuerUidType: z.string(),
    issuerUid: z.string(),
    issuerName: z.string(),
});

export const administrativeBodySchema = z.object({
    name: z.string(),
    bulstat: z.string(),
    identificationNumber: z.string().nullable().optional(),
    headquarters: z.string(),
    address: z.string(),
    email: z.string(),
    phone: z.string(),
});

export type AdministrativeBodyType = z.infer<typeof administrativeBodySchema>;

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

export type UserType = z.infer<typeof userSchema>;

// const AISSchema = z.object({
//     name: z.string(),
//     project: z.string(),
//     sourceIp: z.string(),
//     destinationIp: z.string(),
//     destinationIpType: z.string(),
//     protocolPort: z.string(),
// });

// export type AISType = z.infer<typeof AISSchema>;

export const providerStatus = z.enum(['Denied', 'ReturnedForCorrection', 'Active', 'InReview']);

export type ProviderStatusType = z.infer<typeof providerStatus>;

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

export const providerSubject = z.enum(['Administration', 'PrivateLawSubject']);

export type ProviderSubjectType = z.infer<typeof providerSubject>;

export const providerDetailsSchema = z.object({
    id: z.string(),
    identificationNumber: z.string(),
    name: z.string(),
    uic: z.string(),
    headquarters: z.string(),
    address: z.string(),
});

export const basicProviderSchema = z
    .object({
        id: z.string(),
        type: providerSubject.optional(),
        // aisInformation: AISSchema.nullable(),
        createdOn: z.string(),
        status: providerStatus,
        number: z.string(),
    })
    .extend(issuerSchema.shape)
    .extend(administrativeBodySchema.shape);

export type BasicProviderType = z.infer<typeof basicProviderSchema>;

export const detailedProviderSchema = basicProviderSchema.extend({
    users: z.array(userSchema),
    files: z.array(fileSchema),
    detailsId: z.string().or(z.null()).optional(),
    externalNumber: z.string().or(z.null()),
});

export type DetailedProviderType = z.infer<typeof detailedProviderSchema>;

export const providerStatusHistorySchema = z.object({
    dateTime: z.string(),
    modifierFullName: z.string(),
    status: providerStatus,
    comment: z.string().nullable(),
});

export type ProviderStatusHistoryType = z.infer<typeof providerStatusHistorySchema>;
