import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import {
    AdministratorApplication,
    AdministratorsPageableSchema,
    documentTypeListSchema,
    AttachmentFullSchema,
    RegionListSchema,
    ApplicationDetailsSchema,
} from './administrators.dto';
import { UserService } from '@app/core/services/user.service';
import { ApplicationStatus } from '@app/features/administrators/enums/administrators.enum';

const baseApiUrl = '/raeicei/api/v1';

@Injectable({
    providedIn: 'root',
})
export class AdministratorsService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient, private userService: UserService) {}

    registerAdministrator = (data: AdministratorApplication) => {
        return this.http.post(`${this.apiUrl}/eidadministratorapplication/create`, data, {});
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
            .get(`${this.apiUrl}/eidadministratorapplication/list`, { params: queryParams })
            .pipe(map(data => AdministratorsPageableSchema.parse(data)));
    };

    fetchDocumentTypes = () => {
        return this.http.get(`${this.apiUrl}/document-type/list`).pipe(map(data => documentTypeListSchema.parse(data)));
    };

    fetchApplicationDetails = (id: string) => {
        return this.http
            .get(`${this.apiUrl}/eidadministratorapplication/${id}`)
            .pipe(map(data => ApplicationDetailsSchema.parse(data)));
    };

    downloadFile = (id: string) => {
        return this.http.get(`${this.apiUrl}/document/get/${id}`).pipe(map(data => AttachmentFullSchema.parse(data)));
    };

    fetchRegions = () => {
        return this.http.get(`${this.apiUrl}/region/list`).pipe(map(data => RegionListSchema.parse(data)));
    };

    changeApplicationStatus = (applicationId: string, payload: any, applicationStatus: ApplicationStatus) => {
        return this.http.put(`${this.apiUrl}/application-status/change/${applicationId}`, payload, {
            params: new HttpParams({ fromObject: { applicationStatus: applicationStatus, code: payload.code } }),
        });
    };

    fetchAdministratorCircumstanceChanges = (status: string) => {
        return this.http
            .get(`${this.apiUrl}/newcircs/eidadministrator/findbystatus/${status}`)
            .pipe(map((data: any) => data));
    };

    fetchAdministratorCircumstanceDetails = (id: string) => {
        const queryParams = new HttpParams({ fromObject: { onlyActiveElements: false } });
        return this.http
            .get(`${this.apiUrl}/newcircs/eidadministrator/details/${id}`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    approveAdministratorCircumstanceChange = (id: string, body: any, payload: any) => {
        const queryParams = new HttpParams({ fromObject: payload as any });
        return this.http.put(`${this.apiUrl}/newcircs/eidadministrator/approve/${id}`, body, { params: queryParams });
    };

    fetchAdministrators = () => {
        const queryParams = new HttpParams({ fromObject: { stoppedIncluded: true } });
        return this.http
            .get(`${this.apiUrl}/eidadministrator/history/getAll`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    fetchAdministratorDetails = (id: string, onlyActiveElements = true) => {
        const queryParams = new HttpParams({ fromObject: { onlyActiveElements: onlyActiveElements } });
        return this.http
            .get(`${this.apiUrl}/newcircs/eidadministrator/details/${id}`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    downloadTechnicalCheckFile = () => {
        return this.http.get(`${this.apiUrl}/document/export/technicalcheck`).pipe(map((data: any) => data));
    };
}
