import { CanActivateFn, Router } from '@angular/router';
import jwt_decode from 'jwt-decode';
import { isAdult, isLNCH } from '../../shared/validators/egn';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '../services/user.service';

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const userService = inject(UserService);
    const router = inject(Router);
    const oauthService = inject(OAuthService);

    const token = oauthService.getAccessToken();
    if (!token) {
        oauthService.redirectUri = state.url;
        oauthService.logOut();
        return false;
    }

    let decoded: any = null;
    try {
        decoded = jwt_decode(token);
        userService.setUser(decoded);
        if (decoded.UID && decoded.UIDTYPE === 'EGN' && isAdult(decoded.UID)) {
            return true;
        } else if (decoded.UID && decoded.UIDTYPE === 'LNCh' && isLNCH(decoded.UID)) {
            return true;
        } else {
            authService.removeTokenData();
            return router.parseUrl('/unauthorized');
        }
    } catch (error) {
        authService.removeTokenData();
        return router.parseUrl('/unauthorized');
    }
};
