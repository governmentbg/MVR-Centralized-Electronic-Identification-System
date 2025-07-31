import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin, map } from 'rxjs';
import { MinimumLevelOfAssuranceType, providerServiceSchema, serviceScopeSchema } from './services.dto';
import { ServicesFilterT } from './pages/provider-services/components/services-filter/services-filter.component';
import { createHttpParams } from '@app/shared/utils/create-http-params';
import { PaginationData, SortData } from '@app/shared/types';
import { createPaginatedResponseSchema } from '@app/shared/schemas';
import { z } from 'zod';

const baseUrl = '/admin/api/v1';

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

    fetchProviderServicesDefaultScope = () => {
        return this.http
            .get(`${this.apiUrl}/services/scope/defaults`)
            .pipe(map(data => z.array(z.string()).parse(data)));
    };
    fetchProviderServicesAvailableScope = () => {
        return this.http
            .get(`${this.apiUrl}/services/scope/available`)
            .pipe(map(data => z.array(z.string()).parse(data)));
    };

    fetchProviderServicesFullScope = () => {
        return forkJoin([this.fetchProviderServicesDefaultScope(), this.fetchProviderServicesAvailableScope()]).pipe(
            map(([defaultScope, availableScope]) => [...defaultScope, ...availableScope])
        );
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

    activateService = (serviceId: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/activate`, null);
    };

    deactivateService = (serviceId: string) => {
        return this.http.post(`${this.apiUrl}/services/${serviceId}/deactivate`, null);
    };
}
