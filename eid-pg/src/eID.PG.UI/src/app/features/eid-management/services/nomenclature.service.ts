import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, Observable, shareReplay, tap } from 'rxjs';
import { IReason } from '../interfaces/eid-management.interface';

@Injectable({
    providedIn: 'root',
})
export class NomenclatureService {
    private apiUrl = '/mpozei/external/api/v1';
    _reasonsObservable: any;
    _reasons: IReason[] = [];

    constructor(private http: HttpClient) {
        this.getReasons().subscribe();
    }

    public async reasons() {
        await firstValueFrom(this.getReasons());
        return this._reasons;
    }

    private getReasons(): Observable<IReason[]> {
        if (!this._reasonsObservable) {
            this._reasonsObservable = this.http.get<any>(`${this.apiUrl}/nomenclatures/reasons`).pipe(
                shareReplay(1),
                tap(response => {
                    this._reasons = response;
                })
            );
        }
        return this._reasonsObservable;
    }
}
