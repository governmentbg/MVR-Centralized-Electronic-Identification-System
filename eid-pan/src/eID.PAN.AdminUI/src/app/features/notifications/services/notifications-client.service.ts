import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ISystemPaginatedData, IGetSystemsParams, IRejectedSystemPaginatedData } from '../interfaces/inotification';

@Injectable({
    providedIn: 'root',
})
export class NotificationsClientService {
    private apiUrl = '/api/v1/notifications';

    constructor(private http: HttpClient) {}

    getSystems(params: IGetSystemsParams): Observable<ISystemPaginatedData> {
        const httpParams = new HttpParams()
            .set('pageIndex', params.pageIndex)
            .set('pageSize', params.pageSize)
            .set('includeDeleted', params.includeDeleted || false);

        return this.http.get<ISystemPaginatedData>(`${this.apiUrl}/systems/registered`, {
            params: httpParams,
        });
    }

    getApprovedSystems(params: IGetSystemsParams): Observable<ISystemPaginatedData> {
        const httpParams = new HttpParams().set('pageIndex', params.pageIndex).set('pageSize', params.pageSize);

        return this.http.get<ISystemPaginatedData>(`${this.apiUrl}/systems/registered/approved`, {
            params: httpParams,
        });
    }

    getPendingSystems(params: IGetSystemsParams): Observable<ISystemPaginatedData> {
        const httpParams = new HttpParams().set('pageIndex', params.pageIndex).set('pageSize', params.pageSize);

        return this.http.get<ISystemPaginatedData>(`${this.apiUrl}/systems/registered/pending`, {
            params: httpParams,
        });
    }

    getArchivedSystems(params: IGetSystemsParams): Observable<ISystemPaginatedData> {
        const httpParams = new HttpParams().set('pageIndex', params.pageIndex).set('pageSize', params.pageSize);

        return this.http.get<ISystemPaginatedData>(`${this.apiUrl}/systems/registered/archived`, {
            params: httpParams,
        });
    }

    getRejectedSystems(params: IGetSystemsParams): Observable<IRejectedSystemPaginatedData> {
        const httpParams = new HttpParams().set('pageIndex', params.pageIndex).set('pageSize', params.pageSize);

        return this.http.get<IRejectedSystemPaginatedData>(`${this.apiUrl}/systems/rejected`, {
            params: httpParams,
        });
    }

    modifySystem(system: any, modifiedData: any): Observable<any> {
        return this.http.patch<any>(`${this.apiUrl}/systems/${system.id}`, modifiedData);
    }

    approveSystem(systemId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/systems/${systemId}/approve`, {});
    }

    restoreSystem(systemId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/systems/${systemId}/restore`, {});
    }

    rejectSystem(systemId: string, reason: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/systems/${systemId}/reject`, { reason: reason });
    }

    archiveSystem(systemId: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/systems/${systemId}/archive`, {});
    }
}
