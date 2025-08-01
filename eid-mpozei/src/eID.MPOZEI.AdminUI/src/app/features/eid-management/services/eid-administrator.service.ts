import { Injectable } from '@angular/core';
import { RaeiceiClientService } from './raeicei-client.service';
import { Observable } from 'rxjs';
import { IAdministrator, IAdministratorOffice } from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class EidAdministratorService {
    constructor(private raeiceiClientService: RaeiceiClientService) {}

    private _administrators: IAdministrator[] = [];
    private _allEidFrontOffices: IAdministratorOffice[] = [];
    private _currentAdministratorId = '';
    private _currentAdministratorOfficeId = '';

    get administrators() {
        return this._administrators;
    }

    set administrators(data: IAdministrator[]) {
        this._administrators = data;
    }

    get currentAdministratorId() {
        return this._currentAdministratorId;
    }

    set currentAdministratorId(data: string) {
        this._currentAdministratorId = data;
    }

    get currentAdministratorOfficeId() {
        return this._currentAdministratorOfficeId;
    }

    set currentAdministratorOfficeId(data: string) {
        this._currentAdministratorOfficeId = data;
    }

    get eidFrontOffices() {
        return this._allEidFrontOffices;
    }

    set eidFrontOffices(data: IAdministratorOffice[]) {
        this._allEidFrontOffices = data;
    }

    getAdministratorById(administratorId: string) {
        return this.administrators.find(administrator => administrator.id === administratorId);
    }

    getCurrentAdministratorOffices() {
        return (
            this.administrators.find(administrator => administrator.id === this.currentAdministratorId)
                ?.eidFrontOffices || []
        );
    }

    getOfficeNameById(officeId: string) {
        const foundOffice = this.eidFrontOffices.find(office => {
            return office.id === officeId;
        });
        return foundOffice ? foundOffice.name : null;
    }

    loadAdministrators(): Observable<IAdministrator[]> {
        return new Observable(subscriber => {
            this.raeiceiClientService.getAllAdministrators().subscribe({
                next: response => {
                    this.administrators = response;
                    this.eidFrontOffices = this.administrators.flatMap(item => item.eidFrontOffices || []);
                    subscriber.next(response);
                    subscriber.complete();
                },
                error: error => {
                    subscriber.error(error);
                },
            });
        });
    }
}
