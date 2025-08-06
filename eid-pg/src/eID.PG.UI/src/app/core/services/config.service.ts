import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IAppConfig } from '../interfaces/IAppConfig';

@Injectable({
    providedIn: 'root',
})
export class AppConfigService {
    private http: HttpClient;

    constructor(http: HttpClient) {
        this.http = http;
        this.config = Object.assign({}, this.defaults);
    }

    private defaults: IAppConfig = {
        oauth: {
            issuer: '',
            clientId: '',
            redirectUri: '',
            requireHttps: true,
            responseType: 'code',
            scope: 'openid profile email',
            authUrl: '',
            accessTokenKey: '',
            refreshTokenKey: '',
            provider: '',
            tokenEndpoint: '',
            logoutUrl: '',
            realm: '',
            updateMinValidity: 0,
        },
        externalLinks: {
            paymentUrl: '',
            paymentByAccessCodeUrl: '',
            eDeliveryUrl: '',
            popopUrl: '',
            providersUrl: '',
        },
    };

    config: IAppConfig;

    loadAppConfig() {
        return new Promise<IAppConfig>(resolve => {
            return this.http.get<IAppConfig>('/assets/app.config.json').subscribe({
                next: response => {
                    this.config = Object.assign({}, response);
                    resolve(this.config);
                },
                error: err => {
                    this.config = Object.assign({}, this.defaults);
                    resolve(this.config);
                },
            });
        });
    }
}
