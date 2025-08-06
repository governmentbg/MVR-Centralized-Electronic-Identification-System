import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { IUserNotificationChannelPaginatedData, IUserNotificationChannelParams } from '../interfaces/inotification';

@Injectable({
    providedIn: 'root',
})
export class UserNotificationChannelsClientService {
    private apiUrl = '/pan/api/v1/notificationchannels';

    constructor(private http: HttpClient) {}

    getAll(params: IUserNotificationChannelParams): Observable<IUserNotificationChannelPaginatedData> {
        return this.http.get<IUserNotificationChannelPaginatedData>(this.apiUrl, {
            params: {
                pageSize: params.pageSize,
                pageIndex: params.pageIndex,
            },
        });
    }

    getSelected(): Observable<IUserNotificationChannelPaginatedData> {
        return this.http.get<any>(`${this.apiUrl}/selected`);
    }

    getCombinedNotificationChannels(params: IUserNotificationChannelParams) {
        const getAllNotifications = this.getAll(params);
        const getSelectedNotifications = this.getSelected();

        return forkJoin([getAllNotifications, getSelectedNotifications]);
    }

    doSelection(channelIds: any[]): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/selection`, channelIds);
    }
}
