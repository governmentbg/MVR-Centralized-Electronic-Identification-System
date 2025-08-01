import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class SmartCardCommunicationClientService {
    private apiUrl = 'https://localhost:5025/api/v1';

    constructor(private http: HttpClient) {}

    getReaders(): Observable<string[]> {
        return this.http.get<any>(`${this.apiUrl}/readers`);
    }

    signChallenge(data: { readerName: string; pin: string; can?: string; challenge: string }): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/card/sign-challenge`, data);
    }

    getCurrentVersion(): Observable<{ current: string }> {
        return this.http.get<{ current: string }>(`${this.apiUrl}/version`);
    }
}
