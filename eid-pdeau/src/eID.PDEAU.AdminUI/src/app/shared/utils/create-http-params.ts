import { HttpParams } from '@angular/common/http';

export const createHttpParams = (params: Record<string, any>) => {
    const payload = Object.entries(params).reduce((acc, [key, value]) => {
        if (value || typeof value === 'boolean') {
            acc[key] = value;
        }
        return acc;
    }, {} as Record<string, any>);

    return new HttpParams({ fromObject: payload });
};
