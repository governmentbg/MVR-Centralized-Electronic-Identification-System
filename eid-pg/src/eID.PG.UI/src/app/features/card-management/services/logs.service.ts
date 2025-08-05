import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class LogsService {
    private apiUrl = '/mpozei/external/api/v1';

    constructor(private http: HttpClient) {}

    submitDeviceLog(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/device/log`, data);
    }
}
