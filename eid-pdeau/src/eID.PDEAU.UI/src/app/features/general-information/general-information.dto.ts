import { z } from 'zod';

const officeSchema = z.object({
    name: z.string(),
    address: z.string(),
    lat: z.number(),
    lon: z.number(),
});

export type OfficeType = z.infer<typeof officeSchema>;

const generalInformationSchema = z.string();

export type GeneralInformationType = z.infer<typeof generalInformationSchema>;

export const generalInformationAndOfficesSchema = z.object({
    generalInformation: generalInformationSchema,
    offices: z.array(officeSchema),
});

export type GeneralInformationAndOfficesType = z.infer<typeof generalInformationAndOfficesSchema>;
