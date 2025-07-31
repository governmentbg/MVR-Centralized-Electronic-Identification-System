import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IChannelResponseData, IChannelTestResponseData } from '../interfaces/ichannel';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class ChannelsClientService {
    private apiUrl = '/api/v1/notificationChannels';

    constructor(private http: HttpClient) {}

    getChannels(): Observable<IChannelResponseData> {
        return this.http.get<IChannelResponseData>(this.apiUrl);
    }

    approveChannel(channelId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${channelId}/approve`, {});
    }

    restoreChannel(channelId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${channelId}/restore`, {});
    }

    rejectChannel(channelId: string, reason: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${channelId}/reject`, { reason: reason });
    }

    archiveChannel(channelId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${channelId}/archive`, {});
    }

    testChannel(channelId: string): Observable<IChannelTestResponseData> {
        return this.http.get<IChannelTestResponseData>(`${this.apiUrl}/${channelId}/test`);
    }
}
