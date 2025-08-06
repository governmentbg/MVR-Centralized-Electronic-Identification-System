import { CanActivateFn } from '@angular/router';
import jwt_decode from 'jwt-decode';
import { isAdult, isLNCH } from '../../shared/validators/egn';
import { IdentifierType } from 'src/app/features/authorization-register/enums/authorization-register.enum';
import { inject } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '../services/user.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { TranslocoService } from '@ngneat/transloco';
import { AuthService } from '../services/auth.service';
import { AccessTypes } from 'src/app/shared/enums/access-types';

const lowAssuranceRoutes = ['/profile', '/logs-viewer', '/eid-management', '/notification-settings'];

function isLowAssuranceRoute(url: string): boolean {
    return lowAssuranceRoutes.some(route => url.startsWith(route));
}

function isHighAssuranceValid(decoded: any): boolean {
    if (!decoded.citizen_identifier) {
        return false;
    }

    if (decoded.citizen_identifier_type === IdentifierType.EGN) {
        const isAdultCheck = isAdult(decoded.citizen_identifier);
        return isAdultCheck;
    }

    if (decoded.citizen_identifier_type === IdentifierType.LNCh) {
        const isLNCHCheck = isLNCH(decoded.citizen_identifier);
        return isLNCHCheck;
    }

    return false;
}

function hasAccess(decoded: any, url: string): boolean {
    const isLowAssurance = decoded.acr === AccessTypes.LOW;
    const isHighAssurance = decoded.acr === AccessTypes.HIGH || decoded.acr === AccessTypes.SUBSTANTIAL;

    // Low assurance users can only access specific routes
    if (isLowAssurance) {
        return isLowAssuranceRoute(url);
    }

    // High assurance users can access everything **only if they pass the age checks**
    if (isHighAssurance) {
        const highAssuranceValid = isHighAssuranceValid(decoded);
        return highAssuranceValid;
    }

    return false;
}

export const authGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const userService = inject(UserService);
    const oauthService = inject(OAuthService);
    const toastService = inject(ToastService);
    const translocoService = inject(TranslocoService);
    const token = oauthService.getAccessToken();
    const url = state.url;

    if (!token) {
        oauthService.redirectUri = url;
        authService.removeTokenData();
        oauthService.logOut();
        return false;
    }

    try {
        const decoded: any = jwt_decode(token);
        userService.setUser(decoded);

        if (hasAccess(decoded, url)) {
            return true;
        }
        toastService.showErrorToast(
            translocoService.translate('global.txtErrorTitle'),
            translocoService.translate('global.txtUnauthorized')
        );
        return false;
    } catch (error) {
        toastService.showErrorToast(
            translocoService.translate('global.txtErrorTitle'),
            translocoService.translate('global.txtUnauthorized')
        );
        return false;
    }
};
