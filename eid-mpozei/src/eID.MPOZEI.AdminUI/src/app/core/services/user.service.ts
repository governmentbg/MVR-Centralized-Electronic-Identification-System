import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { RoleType } from '../enums/auth.enum';
import jwt_decode from 'jwt-decode';
import { IUser } from '../interfaces/user';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    constructor(private oAuthService: OAuthService) {}

    private _user: IUser = {
        uid: '',
        uidType: '',
        name: '',
        acr: '',
    };

    set user(data: any) {
        this._user = {
            uid: data['citizen_identifier'],
            uidType: data['citizen_identifier_type'],
            name: [data['given_name'], data['middle_name'], data['family_name']].filter(s => s && s.trim()).join(' '),
            acr: data['acr'],
        };
    }

    get user(): IUser {
        return this._user;
    }

    hasRole(requiredRole: RoleType): boolean {
        return this.getUserRoles().includes(requiredRole);
    }

    getUserRoles() {
        // console.log(this.oAuthService.loadUserProfile());
        let roles: string[] = [];
        const token = this.oAuthService.getAccessToken();
        if (token) {
            const decodedToken: any = jwt_decode(token);
            roles = decodedToken.realm_access.roles || [];
        }
        return roles;
    }
}
