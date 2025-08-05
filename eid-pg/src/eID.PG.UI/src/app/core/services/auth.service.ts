import { Injectable } from '@angular/core';
import { AppConfigService } from './config.service';
import { OAuthStorage } from 'angular-oauth2-oidc';
@Injectable({
    providedIn: 'root',
})
export class AuthService {
    constructor(private appConfigService: AppConfigService, private oAuthStorage: OAuthStorage) {}
    addTokenData(token: string, refreshToken: string) {
        this.oAuthStorage.setItem(this.appConfigService.config.oauth.accessTokenKey, token);
        this.oAuthStorage.setItem(this.appConfigService.config.oauth.refreshTokenKey, refreshToken);
    }
    removeTokenData() {
        this.oAuthStorage.removeItem(this.appConfigService.config.oauth.accessTokenKey);
        this.oAuthStorage.removeItem(this.appConfigService.config.oauth.refreshTokenKey);
        sessionStorage.removeItem('user');
    }
    getTokenData(): { token: string; refreshToken: string } {
        return {
            token: this.oAuthStorage.getItem(this.appConfigService.config.oauth.accessTokenKey) || '',
            refreshToken: this.oAuthStorage.getItem(this.appConfigService.config.oauth.refreshTokenKey) || '',
        };
    }
}
