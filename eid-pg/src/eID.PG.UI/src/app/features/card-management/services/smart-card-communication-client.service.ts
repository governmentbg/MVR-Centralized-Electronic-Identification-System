import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IChangePinRequestData } from '../interfaces/card-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class SmartCardCommunicationClientService {
    private apiUrl = 'https://localhost:5025/api/v1';

    constructor(private http: HttpClient) {}

    getReaders(): Observable<string[]> {
        return this.http.get<any>(`${this.apiUrl}/readers`);
    }

    changePin(data: IChangePinRequestData): Observable<{ certificateSerialNumber: string; message: string }> {
        return this.http.post<any>(`${this.apiUrl}/card/pin/change`, data);
    }

    signChallenge(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/card/sign-challenge`, data);
    }
}
