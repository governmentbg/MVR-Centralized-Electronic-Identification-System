import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    IEmpowermentScopesPayload,
    IEmpowermentServicesPayload,
    IEmpowermentProvidersPayload,
} from '../interfaces/authorization-register.interfaces';
@Injectable({
    providedIn: 'root',
})
export class AuthorizationRegisterService {
    private apiUrl = '/admin/api/v1';

    constructor(private http: HttpClient) {}

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
}
