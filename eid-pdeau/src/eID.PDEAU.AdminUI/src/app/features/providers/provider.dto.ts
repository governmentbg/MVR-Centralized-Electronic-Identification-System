import { z } from 'zod';

const issuerSchema = z.object({
    issuerUidType: z.string(),
    issuerUid: z.string(),
    issuerName: z.string(),
});

export type IssuerType = z.infer<typeof issuerSchema>;

const administrativeBodySchema = z.object({
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

export enum ProviderType {
    Administration = 'Administration',
    PrivateLawSubject = 'PrivateLawSubject',
}

export type FileType = z.infer<typeof fileSchema>;

export const providerSubject = z.nativeEnum(ProviderType);

export type ProviderSubjectType = z.infer<typeof providerSubject>;

const createdByAdminSchema = z.object({
    createdByAdministratorId: z.string().optional().nullable(),
    createdByAdministratorName: z.string().optional().nullable(),
});

export const providerDetailsSchema = z.object({
    id: z.string(),
    identificationNumber: z.string(),
    name: z.string(),
    uic: z.string(),
});

export const basicProviderSchema = z
    .object({
        id: z.string(),
        type: providerSubject.optional(),
        createdOn: z.string(),
        status: providerStatus,
        number: z.string(),
    })
    .extend(issuerSchema.shape)
    .extend(administrativeBodySchema.shape);

export type BasicProviderType = z.infer<typeof basicProviderSchema>;

export const detailedProviderSchema = basicProviderSchema
    .extend({
        users: z.array(userSchema),
        files: z.array(fileSchema),
        detailsId: z.string().optional(),
        number: z.string(),
        externalNumber: z.string().or(z.null()),
    })
    .extend(createdByAdminSchema.shape);

export type DetailedProviderType = z.infer<typeof detailedProviderSchema>;

export const providerStatusHistorySchema = z.object({
    dateTime: z.string(),
    modifierFullName: z.string(),
    status: providerStatus,
    comment: z.string().nullable(),
});

export type ProviderStatusHistoryType = z.infer<typeof providerStatusHistorySchema>;

export function createPaginatedResponseSchema<T extends z.ZodType<any, any>>(itemSchema: T) {
    return z.object({
        data: z.array(itemSchema),
        totalItems: z.number(),
        pageIndex: z.number(),
    });
}
