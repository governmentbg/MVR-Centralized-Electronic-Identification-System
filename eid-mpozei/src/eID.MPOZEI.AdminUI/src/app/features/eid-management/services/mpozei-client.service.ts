import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    IAddGuardiansRequestData,
    IApplication,
    ICertificate,
    ICreateEidApplicationRequestData,
    ICreateEidApplicationResponseData,
    IExportEidApplicationRequestData,
    IFindEidentityByNumberAndTypeRequestData,
    IFindEidentityByNumberAndTypeResponseData,
    IGetApplicationsByEidentityRequestData,
    IGetApplicationsByEidentityResponseData,
    IGetCertificatesByEidentityRequestData,
    IGetCertificatesByEidentityResponseData,
    IHistoryItem,
    IReason,
    ISignApplicationRequestData,
    IUpdateEidApplicationRequestData,
} from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class MpozeiClientService {
    private apiUrl = '/mpozei/api/v1';

    constructor(private http: HttpClient) {}

    findEidentityByNumberAndType(
        data: IFindEidentityByNumberAndTypeRequestData
    ): Observable<IFindEidentityByNumberAndTypeResponseData> {
        const params = new HttpParams({ fromObject: data as any });
        return this.http.get<any>(`${this.apiUrl}/eidentities/find`, { params: params });
    }

    getEidentityById(id: string): Observable<IFindEidentityByNumberAndTypeResponseData> {
        return this.http.get<any>(`${this.apiUrl}/eidentities/${id}`);
    }

    getApplicationsByEidentity(
        data: IGetApplicationsByEidentityRequestData
    ): Observable<IGetApplicationsByEidentityResponseData> {
        const params = new HttpParams({ fromObject: data as any });
        return this.http.get<any>(`${this.apiUrl}/applications/find`, { params: params });
    }

    getCertificatesByEidentity(
        data: IGetCertificatesByEidentityRequestData
    ): Observable<IGetCertificatesByEidentityResponseData> {
        const params = new HttpParams({ fromObject: data as any });
        return this.http.get<any>(`${this.apiUrl}/certificates/find`, { params: params });
    }

    getApplicationById(data: { id: string }): Observable<IApplication> {
        return this.http.get<any>(`${this.apiUrl}/applications/${data.id}`);
    }

    getCertificateById(data: { id: string }): Observable<ICertificate> {
        return this.http.get<any>(`${this.apiUrl}/certificates/${data.id}`);
    }

    createEidApplication(
        data: ICreateEidApplicationRequestData,
        regixAvailability = true
    ): Observable<ICreateEidApplicationResponseData> {
        const params = new HttpParams({ fromObject: { regixAvailability: regixAvailability } });
        return this.http.post<any>(`${this.apiUrl}/applications`, data, { params: params });
    }

    completeApplication(applicationId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications/${applicationId}/complete`, null);
    }

    exportEidApplication(data: IExportEidApplicationRequestData): Observable<any> {
        return this.http.get(`${this.apiUrl}/applications/${data.applicationId}/export`, { responseType: 'blob' });
    }

    exportEidApplicationConfirmation(data: IExportEidApplicationRequestData): Observable<any> {
        return this.http.get(`${this.apiUrl}/applications/${data.applicationId}/export-confirmation`, {
            responseType: 'blob',
        });
    }

    signEidApplication(data: ISignApplicationRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications/${data.id}/sign`, data);
    }

    getCertificateHistory(id: string): Observable<IHistoryItem[]> {
        return this.http.get<any>(`${this.apiUrl}/certificates/${id}/history`);
    }

    denyApplication(data: {
        applicationId: string;
        reason: { reasonId: string; reasonText?: string };
    }): Observable<any> {
        const params = new HttpParams({ fromObject: data.reason as any });
        return this.http.post<any>(
            `${this.apiUrl}/applications/${data.applicationId}/deny`,
            {},
            {
                params: params,
            }
        );
    }

    confirmCertificate(data: {
        applicationId: string;
        status: string;
        reason?: string;
        reasonText?: string;
    }): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/certificates/confirm`, data);
    }

    addGuardians(applicationId: string, data: { guardians: IAddGuardiansRequestData }): Observable<any> {
        return this.http.post(`${this.apiUrl}/applications/${applicationId}/add-guardians`, data, {
            responseType: 'text',
        });
    }

    getAllApplications(data: any): Observable<IGetApplicationsByEidentityResponseData> {
        const params = new HttpParams({ fromObject: data as any });
        return this.http.get<any>(`${this.apiUrl}/applications/admin-find`, { params: params });
    }

    updateEidApplication(data: IUpdateEidApplicationRequestData): Observable<string> {
        const params = new HttpParams({ fromObject: { status: data.status } });
        return this.http.post<any>(`${this.apiUrl}/applications/${data.applicationId}/status`, null, {
            params,
        });
    }

    exportAdminApplications(data: any) {
        const params = new HttpParams({ fromObject: data as any });
        return this.http.get(`${this.apiUrl}/applications/reports/csv/find-applications`, {
            params: params,
            responseType: 'text',
        });
    }
}
