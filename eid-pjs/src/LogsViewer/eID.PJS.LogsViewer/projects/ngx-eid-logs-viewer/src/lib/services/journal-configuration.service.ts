import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IAppConfig } from '../interfaces/IAppConfig';
import { UserService } from './user.service';
import jwt_decode from 'jwt-decode';

@Injectable()
export class AppConfigService {
    constructor(private http: HttpClient, private userService: UserService) {
        // Use this point to store the user information, because
        // this file is loaded on bootstrapping of the parent application
        const token = localStorage.getItem('kc_token');
        if (token) {
            let decodedUserToken: any = jwt_decode(token);
            this.userService.setUser(decodedUserToken);
        }
    }
    private config: IAppConfig = {
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
    };

    loadConfig(): any {
        return this.http
            .get<IAppConfig>('/assets/app.config.json')
            .subscribe((config: IAppConfig) => {
                this.setConfig(config);
            });
    }

    setConfig(config: IAppConfig) {
        this.config = config;
        const integrity = {
            systemId: config.integrity.systemId,
            poolingRequestInterval:
            config.integrity.poolingRequestInterval,
            poolingTimeout: config.integrity.poolingTimeout,
            downloadBytesFileSize: config.integrity.downloadBytesFileSize
        };
        localStorage.setItem('integrity', JSON.stringify(integrity));
    }

    getIntegrityConfig() {
        return this.config.integrity;
    }

    getJournalsConfig() {
        return this.config.journals;
    }
}
