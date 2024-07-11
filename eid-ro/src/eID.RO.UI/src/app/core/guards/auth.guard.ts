import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { KeycloakAuthGuard, KeycloakService } from 'keycloak-angular';
import jwt_decode from 'jwt-decode';
import { isAdult, isLNCH } from '../../shared/validators/egn';
import { AuthService } from '../services/auth.service';
import { IdentifierType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { UserService } from '../services/user.service';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard extends KeycloakAuthGuard {
    constructor(
        protected override readonly router: Router,
        protected readonly keycloak: KeycloakService,
        private authService: AuthService,
        private userService: UserService
    ) {
        super(router, keycloak);
    }

    async isAccessAllowed(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        let decoded: any = null;
        const token = this.keycloak.getKeycloakInstance().token;

        if (!this.authenticated) {
            await this.keycloak.login({
                redirectUri: window.location.origin + state.url,
            });
        }

        if (token) {
            decoded = jwt_decode(token);
            this.userService.setUser(decoded);
            if (decoded.UID && decoded.UIDTYPE === IdentifierType.EGN && isAdult(decoded.UID)) {
                return this.authenticated;
            } else if (decoded.UID && decoded.UIDTYPE === IdentifierType.LNCh && isLNCH(decoded.UID)) {
                return this.authenticated;
            } else {
                this.authService.removeTokenData();
                return this.router.parseUrl('/unauthorized');
            }
        } else {
            this.authService.removeTokenData();
            return this.router.parseUrl('/unauthorized');
        }
    }
}
