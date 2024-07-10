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
        journals: {
            folders: [''],
            fileExtensionFilter: '',
        },
        integrity: {
            systemId: '',
            poolingRequestInterval: 0,
            poolingTimeout: 0,
            downloadBytesFileSize: 0,
        },
        keycloak: {
            keycloakUrl: '',
            keycloakClientId: '',
            keycloakRealm: '',
            keycloakRedirectUri: '',
            keycloakRefreshTokenKey: '',
            keycloakTokenKey: '',
            keycloakUpdateMinValidity: 0,
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
                    console.log('Could not load config file. Using default configuration.');
                    this.config = Object.assign({}, this.defaults);
                    resolve(this.config);
                },
            });
        });
    }
}
