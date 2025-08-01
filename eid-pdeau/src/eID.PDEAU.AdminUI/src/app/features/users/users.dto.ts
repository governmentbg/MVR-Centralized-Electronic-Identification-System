import { z } from 'zod';

const baseUserSchema = z.object({
    uidType: z.string(),
    uid: z.string(),
    name: z.string(),
    email: z.string(),
    isAdministrator: z.boolean(),
    phone: z.string(),
});

export const createUserSchema = baseUserSchema.extend({});
export type CreateUserType = z.infer<typeof createUserSchema>;

export const getUserSchema = baseUserSchema.extend({
    id: z.string(),
    createdOn: z.string(),
});
export type GetUserType = z.infer<typeof getUserSchema>;
