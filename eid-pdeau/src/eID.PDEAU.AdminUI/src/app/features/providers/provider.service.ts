import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import {
    AdministrativeBodyType,
    UserType,
    basicProviderSchema,
    createPaginatedResponseSchema,
    providerDetailsSchema,
    detailedProviderSchema,
    ProviderType,
    IssuerType,
    providerStatusHistorySchema,
} from './provider.dto';
import { PaginationData, SortData } from '@app/shared/types';
import { z } from 'zod';
import { ProviderFilter } from './pages/providers/components/applications-filter/providers-filter.component';
import { createHttpParams } from '@app/shared/utils/create-http-params';
import { administratorActionsHistorySchema, CreateUserType, GetUserType } from '@app/shared/interfaces/users.dto';

@Injectable({
    providedIn: 'root',
})
export class ProviderService {
    private apiUrl = '/api/v1/providers';

    constructor(private http: HttpClient) {}

    fetchProviderApplications = (
        payload: {
            [key: string]: any;
        } & PaginationData &
            SortData &
            ProviderFilter
    ) => {
        const queryParams = createHttpParams(payload);
        return this.http
            .get(this.apiUrl, { params: queryParams })
            .pipe(map(data => createPaginatedResponseSchema(basicProviderSchema).parse(data)));
    };

    fetchProvidersDetails = () => {
        return this.http
            .get(`${this.apiUrl}/details/available`)
            .pipe(map(data => z.array(providerDetailsSchema).parse(data)));
    };

    registerProvider = ({
        issuer,
        providerType,
        administrativeBody,
        administrator,
        files,
        externalNumber,
    }: {
        issuer: IssuerType;
        providerType: ProviderType;
        administrativeBody: AdministrativeBodyType;
        administrator: Omit<UserType, 'id' | 'isAdministrator' | 'createdOn'>;
        files?: File[];
        externalNumber: string;
    }) => {
        const formData = new FormData();
        const payload = {
            issuer,
            ...administrativeBody,
            administrator,
            externalNumber,
        };

        formData.append('data', JSON.stringify(payload));
        if (files?.length) {
            files.forEach(file => {
                formData.append('files', file, file.name);
            });
        }

        return this.http.post(`${this.apiUrl}/${providerType}`, formData, {
            // headers: {
            //     'Content-Type': 'multipart/form-data',
            // },
        });
    };

    fetchProviderById = (providerId: string) => {
        return this.http.get(`${this.apiUrl}/${providerId}`).pipe(map(data => detailedProviderSchema.parse(data)));
    };

    approveProvider = (providerId: string, comment?: string) => {
        return this.http.put(`${this.apiUrl}/${providerId}/approve`, {
            comment,
        });
    };
    denyProvider = (providerId: string, comment?: string) => {
        return this.http.put(`${this.apiUrl}/${providerId}/deny`, {
            comment,
        });
    };
    returnProvider = (providerId: string, comment?: string) => {
        return this.http.put(`${this.apiUrl}/${providerId}/return`, {
            comment,
        });
    };

    downloadFile = ({ fileId, providerId }: { fileId: string; providerId: string; fileName: string }) => {
        return this.http.get(`${this.apiUrl}/${providerId}/files/${fileId}`, { responseType: 'blob' });
    };

    fetchProviderStatusHistory = (providerId: string) => {
        return this.http
            .get(`${this.apiUrl}/${providerId}/statusHistory`)
            .pipe(map(data => providerStatusHistorySchema.array().parse(data)));
    };

    addUser = (providerId: string, user: CreateUserType) => {
        return this.http.post(`${this.apiUrl}/users`, { ...user, providerId });
    };

    updateUser = (providerId: string, user: {isAdministrator: boolean, id: string, comment: string}) => {
        return this.http.put(`${this.apiUrl}/${providerId}/users/${user.id}`, {isAdministrator: user.isAdministrator, comment: user.comment});
    };

    fetchAdministratorActions = (providerId: string, userId: string) => {
        return this.http
            .get(`${this.apiUrl}/${providerId}/users/${userId}/administratorActions`)
            .pipe(map(data => administratorActionsHistorySchema.array().parse(data)));
    };
}
