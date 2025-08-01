import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, map } from 'rxjs';
import { MinimumLevelOfAssuranceType, providerServiceSchema, serviceScopeSchema } from './services.dto';
import { createHttpParams } from '@app/shared/utils/create-http-params';
import { PaginationData, SortData } from '@app/shared/types';
import { createPaginatedResponseSchema } from './services.dto';
import { z } from 'zod';
import { ServicesFilterT } from '@app/features/providers/pages/provider-services/components/services-filter/services-filter.component';

const baseUrl = '/api/v1';

@Injectable({
    providedIn: 'root',
})
export class ProviderServicesService {
    private apiUrl = `${baseUrl}/Providers`;

    constructor(private http: HttpClient) {}

    fetchProviderServices = (
        payload: {
            [key: string]: any;
        } & PaginationData &
            SortData &
            ServicesFilterT
    ) => {
        const queryParams = createHttpParams(payload);
        return this.http
            .get(`${this.apiUrl}/services`, { params: queryParams })
            .pipe(map(data => createPaginatedResponseSchema(providerServiceSchema).parse(data)));
    };

    fetchServicesScope = (serviceId: string) => {
        return this.http
            .get(`${this.apiUrl}/services/${serviceId}/scope`)
            .pipe(map(data => serviceScopeSchema.array().parse(data)));
    };

    editService = (
        serviceId: string,
        payload: {
            description: string;
            providerDetailsId: string;
            isEmpowerment: boolean;
            serviceScopeNames: string[];
            userId: string;
            requiredPersonalInformation: string[];
            minimumLevelOfAssurance: MinimumLevelOfAssuranceType;
        }
    ) => {
        return this.http.put(`${this.apiUrl}/services/${serviceId}`, payload);
    };

    fetchPersonalInformationEnum = () => {
        return this.http
            .get(`${baseUrl}/Nomenclatures/personalinformation`)
            .pipe(map(data => z.array(z.string()).parse(data)));
    };

    addService = (payload: {
        providerDetailsId: string;
        name: string;
        description?: string;
        isEmpowerment: boolean;
        requiredPersonalInformation: string[];
        minimumLevelOfAssurance: MinimumLevelOfAssuranceType;
        serviceScopeNames: string[];
    }) => {
        return this.http.post(`${this.apiUrl}/services`, {
            ...payload,
            paymentInfoNormalCost: 0,
        });
    };

    approveService = (serviceId: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/approve`, null);
    };

    denyService = (serviceId: string, comment: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/deny`, {
            denialReason: comment,
        });
    };

    activateService = (serviceId: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/activate`, null);
    };

    deactivateService = (serviceId: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/deactivate`, null);
    };
}
