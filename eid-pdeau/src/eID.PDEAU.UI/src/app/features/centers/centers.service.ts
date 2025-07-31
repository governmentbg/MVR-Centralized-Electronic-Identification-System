import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import {
    documentTypeListSchema,
    ApplicationDetailsSchema,
    AttachmentFullSchema,
    RegionListSchema,
    CenterApplication,
    CentersPageableSchema,
    CenterSchema, AttachmentsWithNotesSchema,
} from './centers.dto';
import { EmployeesPageableSchema } from '@app/features/administrators/administrators.dto';

const baseApiUrl = '/raeicei/external/api/v1';

@Injectable({
    providedIn: 'root',
})
export class CentersService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient) {}

    registerCenter = (data: CenterApplication) => {
        return this.http.post(`${this.apiUrl}/eidcenterapplication/create`, data, {});
    };

    fetchCenter = () => {
        return this.http.get(`${this.apiUrl}/employee/eidcenter`).pipe(map(data => CenterSchema.parse(data)));
    };

    fetchApplications = (
        payload: {
            [key: string]: any;
        } & {
            page: number;
            size: number;
            sort: string;
        }
    ) => {
        const queryParams = new HttpParams({ fromObject: payload as any });
        return this.http
            .get(`${this.apiUrl}/eidcenterapplication/list`, { params: queryParams })
            .pipe(map(data => CentersPageableSchema.parse(data)));
    };

    fetchDocumentTypes = () => {
        return this.http.get(`${this.apiUrl}/document-type/list`).pipe(map(data => documentTypeListSchema.parse(data)));
    };

    fetchApplicationDetails = (id: string) => {
        return this.http
            .get(`${this.apiUrl}/eidcenterapplication/${id}`)
            .pipe(map(data => ApplicationDetailsSchema.parse(data)));
    };

    downloadFile = (id: string) => {
        return this.http.get(`${this.apiUrl}/document/get/${id}`).pipe(map(data => AttachmentFullSchema.parse(data)));
    };

    fetchOffices = () => {
        return this.http.get(`${this.apiUrl}/employee/eidmanagerfrontoffice/getAll`);
    };

    fetchEmployees = (
        payload: {
            [key: string]: any;
        } & {
            page: number;
            size: number;
            sort: string;
        }
    ) => {
        const queryParams = new HttpParams({ fromObject: payload as any });
        return this.http
            .get(`${this.apiUrl}/employee/employee/list`, { params: queryParams })
            .pipe(map(data => EmployeesPageableSchema.parse(data)));
    };

    fetchRegions = () => {
        return this.http.get(`${this.apiUrl}/region/list`).pipe(map(data => RegionListSchema.parse(data)));
    };

    // Office
    createOffice = (data: any) => {
        return this.http.post(`${this.apiUrl}/circs/eidmanagerfrontoffice/create`, data, {});
    };

    updateOfficeById = (data: any) => {
        return this.http.put(`${this.apiUrl}/circs/eidmanagerfrontoffice/update`, data, {});
    };

    deleteOfficeById = (id: string) => {
        return this.http.delete(`${this.apiUrl}/circs/eidmanagerfrontoffice/delete/${id}`);
    };

    // Employee
    createEmployee = (data: any) => {
        return this.http.post(`${this.apiUrl}/circs/employee/create`, data, {});
    };

    updateEmployeeById = (data: any, id: string) => {
        return this.http.put(`${this.apiUrl}/circs/employee/update/${id}`, data, {});
    };

    deleteEmployeeById = (id: string) => {
        return this.http.delete(`${this.apiUrl}/circs/employee/delete/${id}`);
    };

    // Authorized person
    createAuthorizedPerson = (data: any) => {
        return this.http.post(`${this.apiUrl}/circs/authorizedperson/create`, data, {});
    };

    updateAuthorizedPersonById = (data: any, id: string) => {
        return this.http.put(`${this.apiUrl}/circs/authorizedperson/update/${id}`, data, {});
    };

    deleteAuthorizedPersonById = (id: string) => {
        return this.http.delete(`${this.apiUrl}/circs/authorizedperson/delete/${id}`);
    };

    fetchAttachments = () => {
        return this.http.get(`${this.apiUrl}/circs/eidmanager/documentsnotes`).pipe(map(data => AttachmentsWithNotesSchema.parse(data)));
    }

    createDocument = (data: any) => {
        return this.http.post(`${this.apiUrl}/eidmanager/document/upload`, data, {});
    }

    // Upload document to Application
    uploadDocuments = (data: any, id: string) => {
        return this.http.post(`${this.apiUrl}/application/document/upload/${id}`, data);
    }

    createApplication = (data: any) => {
        return this.http.post(`${this.apiUrl}/eidcenterapplication/create`, data, {});
    };
}
