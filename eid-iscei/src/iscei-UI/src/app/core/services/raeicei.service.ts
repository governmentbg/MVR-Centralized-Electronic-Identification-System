import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, first } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { IEidAdministrator } from '../interfaces/IAdministrator';
import { AppConfigService } from './config.service';

@Injectable({
    providedIn: 'root',
})
export class RaeiceiClientService {
    administratorsList!: IEidAdministrator[];
    private apiUrl = '/raeicei/external/api/v1';
    private pdeauApiUrl = '/pdeau/api/v1';

    constructor(private http: HttpClient, public translateService: TranslocoService, private appConfirgService: AppConfigService) {}
    fetchAdministrators(): Observable<IEidAdministrator[]> {
        return this.http.get<IEidAdministrator[]>(`${this.apiUrl}/eidadministrator/getAll`).pipe(first());
    }

    fetchEidCenters(): Observable<IEidAdministrator[]> {
        return this.http.get<IEidAdministrator[]>(`${this.apiUrl}/eidcenter/getAll`).pipe(first());
    }

    fetchProviders(): Observable<IEidAdministrator[]> {
        return this.http.get<IEidAdministrator[]>(`${this.pdeauApiUrl}/Providers/list`).pipe(first());
    }
}