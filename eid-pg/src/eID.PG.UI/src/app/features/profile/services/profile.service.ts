import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../interfaces/user';
@Injectable({
    providedIn: 'root',
})
export class ProfileService {
    private apiUrl = '/mpozei/external/api/v1';

    constructor(private http: HttpClient) {}

    getUser(): Observable<any> {
        return this.http.get<User>(`${this.apiUrl}/eidentities`);
    }

    updateEmail(email: string): Observable<any> {
        const params = new HttpParams().set('email', email);
        return this.http.post<any>(`${this.apiUrl}/citizens/update-email`, {}, { params });
    }

    updatePersonalInfo(data: any): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/citizens`, data);
    }

    updatePassword(data: any): Observable<any> {
        const params = new HttpParams()
            .set('oldPassword', data.oldPassword)
            .set('newPassword', data.newPassword)
            .set('confirmPassword', data.confirmPassword);
        return this.http.post<User>(`${this.apiUrl}/citizens/update-password`, {}, { params });
    }

    forgottenPassword(payload: any): Observable<any> {
        return this.http.post<User>(`${this.apiUrl}/citizens/forgotten-password`, payload);
    }

    resetPassword(data: any): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.post<User>(`${this.apiUrl}/citizens/reset-password`, {}, { params });
    }

    registerUser(body: User): Observable<any> {
        return this.http.post(`${this.apiUrl}/citizens/register`, body);
    }

    verifyEmail(token: string): Observable<any> {
        return this.http.put(`${this.apiUrl}/citizens/register`, { token: token });
    }

    attachEidentityToProfile(citizenProfileId: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/eidentities/attach-to-profile/${citizenProfileId}`, {});
    }
}
