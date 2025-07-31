import { Injectable } from '@angular/core';
import { AppConfigService } from './config.service';
@Injectable({
    providedIn: 'root',
})
export class AuthService {
    constructor(private appConfigService: AppConfigService) {}
    addTokenData(token: string, refreshToken: string) {
        sessionStorage.setItem(this.appConfigService.config.oauth.accessTokenKey, token);
        sessionStorage.setItem(this.appConfigService.config.oauth.refreshTokenKey, refreshToken);
    }
    removeTokenData() {
        sessionStorage.removeItem(this.appConfigService.config.oauth.accessTokenKey);
        sessionStorage.removeItem(this.appConfigService.config.oauth.refreshTokenKey);
        sessionStorage.removeItem('user');
    }
    getTokenData(): { token: string; refreshToken: string } {
        return {
            token: sessionStorage.getItem(this.appConfigService.config.oauth.accessTokenKey) || '',
            refreshToken: sessionStorage.getItem(this.appConfigService.config.oauth.refreshTokenKey) || '',
        };
    }
}
