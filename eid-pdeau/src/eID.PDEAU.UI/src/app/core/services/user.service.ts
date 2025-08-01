import { Injectable } from '@angular/core';
import { IUser } from '../interfaces/IUser';
import { BehaviorSubject } from 'rxjs';
import { CurrentProviderService } from '@app/core/services/current-provider.service';
import { RoleType, SystemType } from '@app/core/enums/auth.enum';
import jwt_decode from 'jwt-decode';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    constructor(private oAuthService: OAuthService, private currentProviderService: CurrentProviderService) {
        if (this.oAuthService.hasValidAccessToken()) {
            this.setUser(jwt_decode(this.oAuthService.getAccessToken()));
        }
    }
    userSubject = new BehaviorSubject(false);
    public user: IUser = {
        systemId: '',
        systemName: '',
        userId: '',
        locale: '',
        uid: '',
        uidType: '',
        name: '',
        nameLatin: '',
        acr: '',
    };

    setUser(data: any) {
        this.user = {
            systemId: data?.system_id,
            systemName: data?.system_name,
            userId: data?.eidenity_id,
            locale: data?.locale,
            uid: data?.citizen_identifier,
            uidType: data?.citizen_identifier_type,
            name: [data.given_name_cyrillic, data.middle_name_cyrillic, data.family_name_cyrillic]
                .filter(s => s && s.trim())
                .join(' '),
            nameLatin: [data.given_name, data.middle_name, data.family_name].filter(s => s && s.trim()).join(' '),
            acr: data.acr,
        };
    }

    getUser() {
        return this.user;
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

    getSystemType(): SystemType | null {
        const token = this.oAuthService.getAccessToken();
        if (token) {
            const decodedToken: any = jwt_decode(token);
            const scopes: string = decodedToken.scope;
            for (const key in SystemType) {
                const term = SystemType[key as keyof typeof SystemType];
                if (scopes.includes(term)) {
                    return term;
                }
            }
        }
        return null;
    }

    initSystemTypeForUser() {
        const systemType = this.getSystemType();
        if (systemType) {
            switch (systemType) {
                case SystemType.DEAU:
                    if (this.hasRole(RoleType.ADMINISTRATOR)) {
                        this.currentProviderService.init();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
