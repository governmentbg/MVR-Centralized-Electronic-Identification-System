import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root',
})
export class ReportsClientService {
    private apiUrl = '/mpozei/api/v1';

    constructor(private http: HttpClient) {}

    getReportByOperators(data: any) {
        return this.http.get<any>(`${this.apiUrl}/applications/reports/ui/by-operators`, { params: data });
    }

    getReportByOperatorsCSV(data: any) {
        return this.http.get(`${this.apiUrl}/applications/reports/csv/by-operators`, {
            params: data,
            responseType: 'text',
        });
    }
}
