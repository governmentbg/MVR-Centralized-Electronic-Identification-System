import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class PjsClientService {
    private apiUrl = '/pjs/api/v1';

    constructor(private http: HttpClient) {}

    exportLogs(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/log/from/export/csv`, data, {
            observe: 'response',
            responseType: 'blob' as 'json',
        });
    }

    getLogsFrom(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/log/from`, data);
    }
}
