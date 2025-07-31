import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { SystemType } from '@app/core/enums/auth.enum';

export const AuthGuard: CanActivateFn = (route, state) => {
    const oauthService = inject(OAuthService);
    const userService = inject(UserService);
    const router = inject(Router);
    const hasValidAccessToken = oauthService.hasValidAccessToken();

    const requiredRoles = (route.data['roles'] as string[]) || [];
    const requiredSystemTypes = (route.data['systemType'] as SystemType[]) || [];
    const userRoles = userService.getUserRoles();
    const userSystemType = userService.getSystemType();

    if (hasValidAccessToken) {
        if (requiredRoles.length > 0) {
            if (userRoles.some(role => requiredRoles.includes(role))) {
                return true;
            } else {
                return router.createUrlTree(['/unauthorized']);
            }
        } else if (requiredSystemTypes.length > 0) {
            if (userSystemType && requiredSystemTypes.includes(userSystemType)) {
                return true;
            } else {
                return router.createUrlTree(['/unauthorized']);
            }
        } else {
            return true;
        }
    } else {
        oauthService.logOut();
        return hasValidAccessToken;
    }
};
