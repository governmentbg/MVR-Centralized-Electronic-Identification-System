export interface IAppConfig {
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
