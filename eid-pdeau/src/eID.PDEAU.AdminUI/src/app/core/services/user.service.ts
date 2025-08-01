import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { IUser } from '@app/core/interfaces/IUser';
import { RoleType } from '@app/core/enums/auth.enum';
import jwt_decode from 'jwt-decode';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    constructor(private oAuthService: OAuthService) {}

    private _user: IUser = {
        providerId: '',
        userId: '',
        locale: '',
        uid: '',
        uidType: '',
        name: '',
        acr: '',
        eidadministratorid: '',
    };

    set user(data: any) {
        this._user = {
            uid: data['citizen_identifier'],
            uidType: data['citizen_identifier_type'],
            name: [data['given_name'], data['middle_name'], data['family_name']].filter(s => s && s.trim()).join(' '),
            acr: data['acr'],
            locale: data['locale'],
            userId: data['provider_id'],
            providerId: data['provider_id'],
            eidadministratorid: data['eidadministratorid'],
        };
    }

    get user(): IUser {
        return this._user;
    }

    hasRole(requiredRole: RoleType): boolean {
        return this.getUserRoles().includes(requiredRole);
    }

    getUserRoles() {
        let roles: string[] = [];
        const token = this.oAuthService.getAccessToken();
        if (token) {
            const decodedToken: any = jwt_decode(token);
            roles = decodedToken.realm_access.roles || [];
        }
        return roles;
    }
}
