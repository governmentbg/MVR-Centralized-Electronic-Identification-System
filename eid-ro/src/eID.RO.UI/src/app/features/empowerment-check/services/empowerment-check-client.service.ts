import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IEmpowermentCheckRequestData } from '../interfaces/IEmpowermentCheckRequestData';

@Injectable({
    providedIn: 'root',
})
export class EmpowermentCheckClientService {
    private apiUrl = '/api/v1/deau';

    constructor(private http: HttpClient) {}

    empowermentCheck(data: IEmpowermentCheckRequestData): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/empowerments`, data);
    }
}
