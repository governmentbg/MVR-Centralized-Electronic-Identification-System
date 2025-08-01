import { z } from 'zod';

export function createPaginatedResponseSchema<T extends z.ZodType<any, any>>(itemSchema: T) {
    return z.object({
        data: z.array(itemSchema),
        totalItems: z.number(),
        pageIndex: z.number(),
    });
}
