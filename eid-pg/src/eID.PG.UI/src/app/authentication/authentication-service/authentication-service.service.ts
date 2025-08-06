import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { OAuthService } from 'angular-oauth2-oidc';
import { AppConfigService } from 'src/app/core/services/config.service';
enum TokenTypes {
    REFRESH_TOKEN = 'REFRESH_TOKEN',
    AUTHORIZATION_CODE = 'AUTHORIZATION_CODE',
}
@Injectable({
    providedIn: 'root',
})
export class AuthenticationService {
    constructor(
        private http: HttpClient,
        private oauthService: OAuthService,
        private appConfigService: AppConfigService
    ) {}

    private keycloakAuthUrl = '/iscei/api/v1/auth';
    private tokenAuthUrl = '/iscei/api/v1';

    basicLogin(data: any) {
        return this.http.post<any>(`${this.keycloakAuthUrl}/basic`, data);
    }

    getAuthChallenge(data: any) {
        return this.http.post<any>(`${this.keycloakAuthUrl}/generate-authentication-challenge`, data);
    }

    associateProfile(data: any) {
        const params = new HttpParams().set('client_id', this.appConfigService.config.oauth.clientId);
        return this.http.post<any>(`${this.keycloakAuthUrl}/associate-profiles`, data, { params });
    }

    refreshToken(client_id: string) {
        const refreshToken = this.oauthService.getRefreshToken();
        const params = new HttpParams()
            .set('client_id', client_id)
            .set('grant_type', TokenTypes.REFRESH_TOKEN)
            .set('refresh_token', refreshToken);
        return this.http.get<any>(`${this.tokenAuthUrl}/code-flow/token`, { params });
    }
}
