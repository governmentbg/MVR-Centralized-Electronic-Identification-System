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
    private apiUrl = '/ro/api/v1';

    constructor(private http: HttpClient) {}

    addEmpowerment(empowermentRequestData: IEmpowermentRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments`, empowermentRequestData);
    }

    withdrawEmpowerment(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/${data.id}/withdraw`, { reason: data.reason });
    }

    getWithdrawalReasons(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/empowerments/withdraw/reasons`);
    }

    declareDisagreementEmpowerment(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/${data.id}/disagreement`, { reason: data.reason });
    }

    getDisagreementReasons(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/empowerments/disagreement/reasons`);
    }

    signEmpowerment(data: SendSignatureRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/${data.empowermentId}/sign`, {
            detachedSignature: data.detachedSignature,
            signatureProvider: data.signatureProvider,
        });
    }

    getEmpowermentsTo(params: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/empowerments/to`, { params: params });
    }

    getEmpowermentsFrom(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/from`, data);
    }

    getProviders(data: IEmpowermentProvidersPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/providers`, { params: params });
    }

    getServices(data: IEmpowermentServicesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/providers/services`, { params: params });
    }

    getScopesByService(data: IEmpowermentScopesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/providers/services/${data.serviceId}/scope`, { params: params });
    }

    getEmpowermentsForLegalEntity(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments/eik`, data);
    }

    getEmpowermentFromMe(data: any): Observable<any> {
        return this.http.get(`${this.apiUrl}/empowerments/${data.empowermentId}/document/from`, {
            responseType: 'blob',
        });
    }

    getEmpowermentToMe(data: any): Observable<any> {
        return this.http.get(`${this.apiUrl}/empowerments/${data.empowermentId}/document/to`, { responseType: 'blob' });
    }
}
