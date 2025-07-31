import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IDevice, IAdministratorOffice } from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class RaeiceiClientService {
    private apiUrl = '/raeicei/api/v1';

    constructor(private http: HttpClient) {}

    getAllDevices(): Observable<IDevice[]> {
        return this.http.get<any>(`${this.apiUrl}/device/getAll`);
    }

    getAllAdministrators(): Observable<any[]> {
        return this.http.get<any>(`${this.apiUrl}/eidadministrator/getAll`);
    }

    getAdministratorOffices(administratorId: string): Observable<IAdministratorOffice[]> {
        return this.http.get<IAdministratorOffice[]>(`${this.apiUrl}/eidmanagerfrontoffice/getAll/${administratorId}`);
    }
}
