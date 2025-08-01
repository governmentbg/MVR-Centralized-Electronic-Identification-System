import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ICarrier } from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class PunClientService {
    private apiUrl = '/pun/api/v1';

    constructor(private http: HttpClient) {}

    getCarriers(data: {
        serialNumber?: string;
        eId?: string;
        certificateId?: string;
        type?: string;
    }): Observable<ICarrier[]> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/carriers`, { params: params });
    }
}
