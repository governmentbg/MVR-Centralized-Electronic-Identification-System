import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
    providedIn: 'root',
})
export class LandingPagesService {
    private apiUrl = '/mpozei/external/api/v1';

    constructor(private http: HttpClient) {}

    activateProfile(token: string): Observable<any> {
        const params = new HttpParams({ fromObject: { token: token } });
        return this.http.put(`${this.apiUrl}/citizens/register`, {}, { params });
    }

    updateEmail(token: string): Observable<any> {
        const params = new HttpParams({ fromObject: { token: token } });
        return this.http.post(`${this.apiUrl}/citizens/update-email/confirm`, {}, { params });
    }
}
