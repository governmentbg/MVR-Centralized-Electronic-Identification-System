import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import {
    AdministrativeBodyType,
    UserType,
    ProviderSubjectType,
    providerDetailsSchema,
    basicProviderSchema,
    detailedProviderSchema,
    providerStatusHistorySchema,
} from './provider.dto';
import { ApplicationFilter } from './pages/provider-applications/components/applications-filter/applications-filter.component';
import { createHttpParams } from '@app/shared/utils/create-http-params';

import { PaginationData, SortData } from '@app/shared/types';
import { createPaginatedResponseSchema } from '@app/shared/schemas';
import { z } from 'zod';
import { UserService } from '@app/core/services/user.service';
import { RoleType } from '@app/core/enums/auth.enum';

const baseApiUrl = '/api/v1/Providers';

@Injectable({
    providedIn: 'root',
})
export class ProviderService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient, private userService: UserService) {
        const isEmployee = this.userService.hasRole(RoleType.EMPLOYEE);

        if (isEmployee) {
            this.apiUrl = '/admin' + baseApiUrl;
        }
    }

    registerProvider = ({
        providerType,
        administrativeBody,
        administrator,
        files,
    }: {
        providerType: ProviderSubjectType;
        administrativeBody: AdministrativeBodyType;
        administrator: Omit<UserType, 'id' | 'isAdministrator' | 'createdOn'>;
        files?: File[];
    }) => {
        const formData = new FormData();
        const payload = {
            ...administrativeBody,
            administrator,
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

    updateProvider = ({
        providerId,
        administrativeBody,
        administrator,
        files,
        comment,
    }: {
        providerId: string;
        administrativeBody: AdministrativeBodyType;
        administrator: Omit<UserType, 'id' | 'isAdministrator' | 'createdOn'>;
        files?: File[];
        comment: string;
    }) => {
        const formData = new FormData();
        const payload = {
            ...administrativeBody,
            administrator,
            comment,
        };

        formData.append('data', JSON.stringify(payload));
        if (files?.length) {
            files.forEach(file => {
                formData.append('files', file, file.name);
            });
        }

        return this.http.put(`${this.apiUrl}/${providerId}`, formData);
    };

    fetchProviderApplications = (
        payload: {
            [key: string]: any;
        } & PaginationData &
            SortData &
            ApplicationFilter
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

    fetchProviderById = (providerId: string) => {
        return this.http.get(`${this.apiUrl}/${providerId}`).pipe(map(data => detailedProviderSchema.parse(data)));
    };

    downloadFile = (fileId: string, fileName: string) => {
        return this.http.get(`${this.apiUrl}/files/${fileId}`, { responseType: 'blob' });
    };

    fetchProviderStatusHistory = (providerId: string) => {
        return this.http
            .get(`${baseApiUrl}/${providerId}/statushistory`)
            .pipe(map(data => providerStatusHistorySchema.array().parse(data)));
    };
}
