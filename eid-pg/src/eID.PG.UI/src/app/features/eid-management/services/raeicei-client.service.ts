import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, first } from 'rxjs';
import { TranslocoService } from '@ngneat/transloco';
import { IEidAdministrator, IDevices, IEidAdministratorOffice } from '../interfaces/eid-management.interface';
import { FormBuilder } from '@angular/forms';

@Injectable({
    providedIn: 'root',
})
export class RaeiceiClientService {
    administratorsList!: IEidAdministrator[];
    devicesList!: IDevices[];
    administratorOffice!: IEidAdministratorOffice[];
    private apiUrl = '/raeicei/external/api/v1';

    constructor(private http: HttpClient, public translateService: TranslocoService) {}
    calculateTariff(data: FormBuilder): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/calculateTariff`, data);
    }

    getProvidedService(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/providedservice/getAll`);
    }

    getAdministratorById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/eidadministrator/${id}`);
    }

    getDeviceById(id: string): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/device/${id}`);
    }

    getEidCenterById(id: string): Observable<IEidAdministrator> {
        return this.http.get<IEidAdministrator>(`${this.apiUrl}/eidcenter/${id}`).pipe(first());
    }

    fetchAdministrators(): Observable<IEidAdministrator[]> {
        return this.http.get<IEidAdministrator[]>(`${this.apiUrl}/eidadministrator/getAll`).pipe(first());
    }

    fetchEidCenters(): Observable<IEidAdministrator[]> {
        return this.http.get<IEidAdministrator[]>(`${this.apiUrl}/eidcenter/getAll`).pipe(first());
    }

    fetchDevices(): Observable<IDevices[]> {
        return this.http.get<IDevices[]>(`${this.apiUrl}/device/getAll`).pipe(first());
    }

    fetchOffices(administratorId: string): Observable<IEidAdministratorOffice[]> {
        return this.http
            .get<IEidAdministratorOffice[]>(`${this.apiUrl}/eidmanagerfrontoffice/getAll/${administratorId}`)
            .pipe(first());
    }
}
