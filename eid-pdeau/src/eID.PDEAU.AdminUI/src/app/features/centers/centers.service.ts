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
} from './centers.dto';
import { ApplicationStatus } from '@app/features/centers/enums/centers.enum';

const baseApiUrl = '/raeicei/api/v1';

@Injectable({
    providedIn: 'root',
})
export class CentersService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient) {}

    registerCenter = (data: CenterApplication) => {
        return this.http.post(`${this.apiUrl}/eidcenterapplication/create`, data, {});
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

    fetchRegions = () => {
        return this.http.get(`${this.apiUrl}/region/list`).pipe(map(data => RegionListSchema.parse(data)));
    };

    changeApplicationStatus = (applicationId: string, payload: any, applicationStatus: ApplicationStatus) => {
        return this.http.put(`${this.apiUrl}/application-status/change/${applicationId}`, payload, {
            params: new HttpParams({ fromObject: { applicationStatus: applicationStatus, code: payload.code } }),
        });
    };

    fetchCenterCircumstanceChanges = (status: string) => {
        return this.http.get(`${this.apiUrl}/newcircs/eidcenter/findbystatus/${status}`).pipe(map((data: any) => data));
    };

    fetchCenterCircumstanceDetails = (id: string) => {
        const queryParams = new HttpParams({ fromObject: { onlyActiveElements: false } });
        return this.http
            .get(`${this.apiUrl}/newcircs/eidcenter/details/${id}`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    approveCenterCircumstanceChange = (id: string, body: any, payload: any) => {
        const queryParams = new HttpParams({ fromObject: payload as any });
        return this.http.put(`${this.apiUrl}/newcircs/eidcenter/approve/${id}`, body, { params: queryParams });
    };

    fetchCenters = () => {
        const queryParams = new HttpParams({ fromObject: { stoppedIncluded: true } });
        return this.http
            .get(`${this.apiUrl}/eidcenter/history/getAll`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    fetchCenterDetails = (id: string, onlyActiveElements = true) => {
        const queryParams = new HttpParams({ fromObject: { onlyActiveElements: onlyActiveElements } });
        return this.http
            .get(`${this.apiUrl}/newcircs/eidcenter/details/${id}`, { params: queryParams })
            .pipe(map((data: any) => data));
    };

    downloadTechnicalCheckFile = () => {
        return this.http.get(`${this.apiUrl}/document/export/technicalcheck`).pipe(map((data: any) => data));
    };
}
