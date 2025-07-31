import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

const baseApiUrl = '/iscei/api/v1';
@Injectable({
    providedIn: 'root',
})
export class StatisticsService {
    private apiUrl = baseApiUrl;

    constructor(private http: HttpClient) {}

    fetchRequestsCount = (data: { createDateFrom: string; createDateTo: string; systemId: string }) => {
        return this.http
            .get(`${this.apiUrl}/statistics/report/pui/requests-count`, { params: data })
            .pipe(map((data: any) => data));
    };

    downloadRequestsCount = (data: { createDateFrom: string; createDateTo: string; systemId: string }) => {
        return this.http.get(`${this.apiUrl}/statistics/report/csv/requests-count`, {
            params: data,
            responseType: 'text',
        });
    };
}
