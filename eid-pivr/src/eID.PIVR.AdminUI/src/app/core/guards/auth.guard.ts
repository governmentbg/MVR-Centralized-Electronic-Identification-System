import { CanActivateFn, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { inject } from '@angular/core';
import { UserService } from '../services/user.service';

export const AuthGuard: CanActivateFn = (route, state) => {
    const oauthService = inject(OAuthService);
    const userService = inject(UserService);
    const router = inject(Router);
    const hasValidAccessToken = oauthService.hasValidAccessToken();

    const requiredRoles = (route.data['roles'] as string[]) || [];
    const userRoles = userService.getUserRoles();

    if (requiredRoles.length > 0) {
        if (userRoles.some(role => requiredRoles.includes(role))) {
            return true;
        } else {
            return router.createUrlTree(['/unauthorized']);
        }
    } else {
        return hasValidAccessToken;
    }
};
