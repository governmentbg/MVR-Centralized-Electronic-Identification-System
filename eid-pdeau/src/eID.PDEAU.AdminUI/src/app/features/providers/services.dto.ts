import { z } from 'zod';
import { basicProviderSchema } from '@app/features/providers/provider.dto';

export const minimumLevelOfAssuranceEnum = z.enum(['High', 'Substantial', 'Low', 'None']);

export type MinimumLevelOfAssuranceType = z.infer<typeof minimumLevelOfAssuranceEnum>;

export enum providerServiceStatusEnum {
    None = 'None',
    InReview = 'InReview',
    Approved = 'Approved',
    Denied = 'Denied',
}

export const providerServiceSchema = z.object({
    id: z.string(),
    serviceNumber: z.number(),
    name: z.string(),
    description: z.string().optional().nullable(),
    isEmpowerment: z.boolean(),
    minimumLevelOfAssurance: minimumLevelOfAssuranceEnum,
    requiredPersonalInformation: z.array(z.string()).nullable(),
    providerDetailsId: z.string(),
    status: z.nativeEnum(providerServiceStatusEnum).optional(),
    isActive: z.boolean().optional().nullable(),
    lastModifiedOn: z.string().optional().nullable(),
    createdOn: z.string(),
    denialReason: z.string().nullable(),
    provider: basicProviderSchema.optional().nullable(),
    deniedOn: z.string().nullable(),
});

export type ProviderServiceType = z.infer<typeof providerServiceSchema>;

export const serviceScopeSchema = z.object({
    id: z.string(),
    name: z.string(),
});

export type ServiceScopeType = z.infer<typeof serviceScopeSchema>;

export function createPaginatedResponseSchema<T extends z.ZodType<any, any>>(itemSchema: T) {
    return z.object({
        data: z.array(itemSchema),
        totalItems: z.number(),
        pageIndex: z.number(),
    });
}

export enum ToggleAction {
    Activate = 'Activate',
    Deactivate = 'Deactivate',
}
