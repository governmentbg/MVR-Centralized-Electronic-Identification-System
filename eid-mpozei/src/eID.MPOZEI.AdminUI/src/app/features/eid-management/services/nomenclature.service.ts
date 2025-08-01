import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { shareReplay, tap, catchError } from 'rxjs/operators';
import { IReason } from '../interfaces/eid-management.interfaces';

@Injectable({
    providedIn: 'root',
})
export class NomenclatureService {
    private apiUrl = '/mpozei/api/v1/nomenclatures/reasons';
    private reasons$: Observable<IReason[]>;

    constructor(private http: HttpClient) {
        this.reasons$ = this.fetchReasons();
    }

    public getReasons(): Observable<IReason[]> {
        return this.reasons$;
    }

    private fetchReasons(): Observable<IReason[]> {
        return this.http.get<IReason[]>(this.apiUrl).pipe(
            shareReplay(1),
            tap(reasons => (this.reasons$ = of(reasons))),
            catchError(() => of([])) // Handle errors gracefully
        );
    }
}
