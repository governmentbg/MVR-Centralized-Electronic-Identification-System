import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    IEmpowermentRequestData,
    IEmpowermentScopesPayload,
    IEmpowermentServicesPayload,
    IEmpowermentProvidersPayload,
} from '../interfaces/authorization-register.interfaces';
import { SendSignatureRequestData } from '../interfaces/document-sign.interfaces';

@Injectable({
    providedIn: 'root',
})
export class AuthorizationRegisterService {
    private apiUrl = '/api/v1';

    constructor(private http: HttpClient) {}

    withdrawEmpowerment(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/${data.id}/withdraw`, { reason: data.reason });
    }

    declareDisagreementEmpowerment(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/${data.id}/disagreement`, { reason: data.reason });
    }

    getEmpowerments(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments`, data);
    }
    exportHtml(data: any): Observable<any> {
        return this.http.post(
            `${this.apiUrl}/empowerments/${data.id}/export`,
            { html: data.html },
            { responseType: 'blob' }
        );
    }
}
