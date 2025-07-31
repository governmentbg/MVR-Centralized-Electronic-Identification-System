import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { AppConfigService } from './journal-configuration.service';
import { IJournalsConfig } from '../interfaces/IAppConfig';

@Injectable({
    providedIn: 'root',
})
export class JournalsClientService {
    private apiUrl = 'logssearch/api/v1/Search';

    private payload: any;

    private cachedFiles$!: Observable<any>;
    private cachedLogs$!: Observable<any>;

    fileDateRangeFilter = {
        fromDate: new Date(new Date().setDate(new Date().getDate() - 5)),
        toDate: new Date(),
    };

    get fromDate() {
        return this.fileDateRangeFilter.fromDate;
    }

    set fromDate(value: any) {
        this.fileDateRangeFilter.fromDate = value;
    }

    get toDate() {
        return this.fileDateRangeFilter.toDate;
    }

    set toDate(value: any) {
        this.fileDateRangeFilter.toDate = value;
    }

    constructor(
        private http: HttpClient,
        private appConfigService: AppConfigService
    ) {
        const config: IJournalsConfig =
            this.appConfigService.getJournalsConfig();

        this.payload = {
            folders: config.folders,
            fileExtensionFilter: config.fileExtensionFilter,
        };
    }

    getCachedFiles(shouldRefresh?: boolean) {
        if (!this.cachedFiles$ || shouldRefresh) {
            this.cachedFiles$ = this.getFiles().pipe(shareReplay(1));
        }
        return this.cachedFiles$;
    }

    getFiles(): Observable<any> {
        (this.fileDateRangeFilter.fromDate as any) = new Date(
            this.fileDateRangeFilter.fromDate
        ).toISOString();
        (this.fileDateRangeFilter.toDate as any) = new Date(
            this.fileDateRangeFilter.toDate
        ).toISOString();
        this.payload.fileDateRangeFilter = this.fileDateRangeFilter;
        return this.http.post<any>(`${this.apiUrl}/files`, this.payload);
    }

    getFile(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/file`, data);
    }

    getLogs(data: any): Observable<any> {
        this.payload.fileDateRangeFilter = data.fileDateRangeFilter;
        this.payload.dataQuery = data.dataQuery;
        return this.http.post<any>(`${this.apiUrl}/logs`, this.payload);
    }

    getCachedLogs(data: any, shouldRefresh?: boolean) {
        if (!this.cachedLogs$ || shouldRefresh) {
            this.cachedLogs$ = this.getLogs(data).pipe(shareReplay(1));
        }
        return this.cachedLogs$;
    }

    getEstimate(data: any): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/estimate`, data);
    }
}
