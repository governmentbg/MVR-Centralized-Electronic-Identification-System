import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class UserNotificationsClientService {
    private apiUrl = '/pan/api/v1/notifications';

    constructor(private http: HttpClient) {}

    getAll(params: any): Observable<any> {
        return this.http.get<any>(this.apiUrl, {
            params: {
                pageSize: params.pageSize,
                pageIndex: params.pageIndex,
            },
        });
    }

    getDeactivated(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/deactivated`);
    }

    getCombinedUserNotifications(params: any) {
        const getAllNotifications = this.getAll(params);
        const getDeactivated = this.getDeactivated();

        return forkJoin([getAllNotifications, getDeactivated]);
    }

    deactivate(deactivatedNotificationsIds: string[]): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/deactivate`, deactivatedNotificationsIds);
    }
}
