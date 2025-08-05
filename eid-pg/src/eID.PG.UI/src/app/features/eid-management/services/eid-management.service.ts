import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class EidManagementService {
    private apiUrl = '/mpozei/external/api/v1';

    constructor(private http: HttpClient) {}

    generateApplicationXml(data: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/applications/generate-xml`, data, { responseType: 'text' });
    }

    createApplication(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications`, data);
    }

    getAdministratorsList(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/administrators`);
    }

    getEidentity(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/eidentities`);
    }

    getApplicationsList(params: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/applications/find`, { params: params });
    }

    getApplicationById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/applications/${id}`);
    }

    getCertificatesList(params: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/certificates/find`, { params: params });
    }

    getCertificateById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/certificates/${id}`);
    }

    certificateStatusChangePlain(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications/certificate-status-change/plain`, data);
    }

    certificateStatusChangeSigned(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications/certificate-status-change/signed`, data);
    }

    completeCertificate(applicationId: string): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/applications/${applicationId}/complete`, {});
    }

    getCertificateHistory(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/certificates/${id}/history`);
    }

    getDeviceTypes(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/device-types`);
    }

    getHelpPagesList(payload: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/find`, { params: payload });
    }

    getAllHelpPages(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/find-all`);
    }

    getHelpPageById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/${id}`);
    }

    getHelpPageByPageName(pageName: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/help-pages/name/${pageName}`);
    }

    getEidReferences(params: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/certificates/public-find`, { params: params });
    }

    getPaymentHistory(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/payments`);
    }

    updateAlias(alias: string, certificateId: string): Observable<any> {
        const params = new HttpParams().set('certificateId', certificateId);
        return this.http.put<any>(
            `${this.apiUrl}/certificates/alias`,
            {
                alias: alias,
            },
            { params }
        );
    }
}
