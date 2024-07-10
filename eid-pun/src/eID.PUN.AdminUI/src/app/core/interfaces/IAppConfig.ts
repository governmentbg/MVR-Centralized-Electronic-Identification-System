export interface IAppConfig {
    journals: IJournalsConfig;
    integrity: IIntegrityConfig;
    keycloak: {
        keycloakUrl: string;
        keycloakRealm: string;
        keycloakClientId: string;
        keycloakRedirectUri: string;
        keycloakTokenKey: string;
        keycloakRefreshTokenKey: string;
        keycloakUpdateMinValidity: number;
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
