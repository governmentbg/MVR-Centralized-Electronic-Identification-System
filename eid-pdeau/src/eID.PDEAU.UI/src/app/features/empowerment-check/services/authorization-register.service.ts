import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IEmpowermentScopesPayload, IEmpowermentServicesPayload } from '../interfaces/empowerment-check.interfaces';
import { RoleType } from '@app/core/enums/auth.enum';
import { UserService } from '@app/core/services/user.service';
@Injectable({
    providedIn: 'root',
})
export class AuthorizationRegisterService {
    private apiUrl = '/ro/api/v1';

    constructor(private http: HttpClient, private userService: UserService) {
        const isAdmin = this.userService.hasRole(RoleType.ADMINISTRATOR);
        if (isAdmin) {
            this.apiUrl = '/admin/api/v1';
        }
    }

    getServices(data: IEmpowermentServicesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/providers/services`, { params: params });
    }

    getScopesByService(data: IEmpowermentScopesPayload): Observable<any> {
        const params = new HttpParams({ fromObject: data });
        return this.http.get<any>(`${this.apiUrl}/providers/services/${data.serviceId}/scope`, { params: params });
    }
}
