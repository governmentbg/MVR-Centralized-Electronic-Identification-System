import { z } from 'zod';

export const tariffSchema = z.object({
    id: z.string(),
    startDate: z.string(),
    providedServiceId: z.nullable(z.string()),
    primaryPrice: z.number(),
    primaryCurrency: z.string(),
    secondaryPrice: z.number(),
    secondaryCurrency: z.string(),
});

export const TariffListSchema = z.array(tariffSchema);

export type TariffType = z.infer<typeof tariffSchema>;

export const discountSchema = z.object({
    id: z.string(),
    startDate: z.string(),
    ageFrom: z.number(),
    ageUntil: z.number(),
    discount: z.nullable(z.number()),
    disability: z.boolean(),
    onlineService: z.boolean(),
    providedServiceId: z.string(),
});

export const DiscountListSchema = z.array(discountSchema);

export type DiscountType = z.infer<typeof discountSchema>;

export const providedServiceSchema = z.object({
    id: z.string(),
    name: z.string(),
    nameLatin: z.string(),
    applicationType: z.nullable(z.string()),
    serviceType: z.string(),
});

export const ProvidedServiceListSchema = z.array(providedServiceSchema);

export type ProvidedServiceType = z.infer<typeof providedServiceSchema>;
