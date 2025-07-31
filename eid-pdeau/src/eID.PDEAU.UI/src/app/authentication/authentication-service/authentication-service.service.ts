import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    constructor(private http: HttpClient) {}

    private keycloakAuthUrl = '/iscei/api/v1/auth';

    basicLogin(data: any) {
        return this.http.post<any>(`${this.keycloakAuthUrl}/basic`, data);
    }
}
