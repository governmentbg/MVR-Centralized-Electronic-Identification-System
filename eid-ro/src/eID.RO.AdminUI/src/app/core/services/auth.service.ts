import { Injectable } from '@angular/core';
import { AppConfigService } from './config.service';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    constructor(private appConfigService: AppConfigService) {}
    addTokenData(token: string, refreshToken: string) {
        localStorage.setItem(this.appConfigService.config.keycloak.keycloakTokenKey, token);
        localStorage.setItem(this.appConfigService.config.keycloak.keycloakRefreshTokenKey, refreshToken);
    }

    removeTokenData() {
        localStorage.removeItem(this.appConfigService.config.keycloak.keycloakTokenKey);
        localStorage.removeItem(this.appConfigService.config.keycloak.keycloakRefreshTokenKey);
    }

    getTokenData(): { token: string; refreshToken: string } {
        return {
            token: localStorage.getItem(this.appConfigService.config.keycloak.keycloakTokenKey) || '',
            refreshToken: localStorage.getItem(this.appConfigService.config.keycloak.keycloakRefreshTokenKey) || '',
        };
    }
}
