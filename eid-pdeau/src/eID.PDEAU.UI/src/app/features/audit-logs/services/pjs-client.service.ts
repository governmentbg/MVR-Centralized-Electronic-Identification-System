import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class PjsClientService {
    private apiUrl = '/pjs/api/v1';

    constructor(private http: HttpClient) {}

    getLogsFrom(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/log/deau`, data);
    }
}
