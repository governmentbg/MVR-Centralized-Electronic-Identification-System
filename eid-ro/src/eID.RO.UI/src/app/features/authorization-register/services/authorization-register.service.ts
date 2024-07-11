import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
    IEmpowermentScopesPayload,
    IEmpowermentServicesPayload,
    IEmpowermentSuppliersPayload,
} from '../interfaces/authorization-register.interfaces';
@Injectable({
    providedIn: 'root',
})
export class AuthorizationRegisterService {
    private apiUrl = 'api/v1';

    constructor(private http: HttpClient) {}

    getSuppliers(data: IEmpowermentSuppliersPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/suppliers/batches`, { params: params });
    }

    getServices(data: IEmpowermentServicesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/suppliers/services`, { params: params });
    }

    getScopesByService(data: IEmpowermentScopesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/suppliers/services/${data.serviceId}/scope`, { params: params });
    }
}
