import { CanActivateFn } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { inject } from '@angular/core';

export const AuthGuard: CanActivateFn = (route, state) => {
    const oauthService = inject(OAuthService);
    return oauthService.hasValidAccessToken();
};
