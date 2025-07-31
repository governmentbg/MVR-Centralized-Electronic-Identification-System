export interface IAppConfig {
    journals: IJournalsConfig;
    integrity: IIntegrityConfig;
    systemProcesses: {
        prometheus: string;
        grafana: string;
        [name: string]: string;
        keycloak_internal: string;
        keycloak_external: string;
    };
    oauth: {
        issuer: string;
        clientId: string;
        redirectUri: string;
        requireHttps: boolean;
        responseType: string;
        scope: string;
        useSilentRefresh: boolean;
        timeoutFactor: number;
    };
}
export interface IAppConfig {
    journals: IJournalsConfig;
    integrity: IIntegrityConfig;
}
export interface IJournalsConfig {
    fileExtensionFilter?: string;
    folders?: Array<string>;
}
export interface IIntegrityConfig {
    systemId?: string;
    poolingRequestInterval?: number;
    poolingTimeout?: number;
    downloadBytesFileSize: number;
}
