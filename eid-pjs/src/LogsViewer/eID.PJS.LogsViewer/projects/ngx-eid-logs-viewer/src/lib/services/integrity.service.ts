import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class IntegrityClientService {
    private apiUrl = 'pjsservices/api/v1/management';
    constructor(private http: HttpClient) {}

    getVerificationStatus(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/verification-status`);
    }

    checkIntegrityByPeriod(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/verify/period`, data);
    }

    getResult(data: any): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/verify/result`, {
            params: { taskId: data.taskId },
            observe: 'response',
        });
    }

    getResultFile(filePath: string): Observable<any> {
        return this.http.post<any>(
            `${this.apiUrl}/verify/result-file`,
            `\"${filePath}\"`,
            {
                headers: new HttpHeaders({
                    'Content-Type': 'application/json',
                }),
                observe: 'response',
            }
        );
    }
}
