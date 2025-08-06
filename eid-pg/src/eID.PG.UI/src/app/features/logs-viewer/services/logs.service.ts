import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IGetLogs } from '../interfaces/logs.interfaces';

@Injectable({
    providedIn: 'root',
})
export class LogsService {
    private apiUrl = '/pjs/api/v1/log';

    constructor(private http: HttpClient) {}

    getLogsTo(data: IGetLogs): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/to`, data);
    }

    getLogsFrom(data: IGetLogs): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/from`, data);
    }
}
