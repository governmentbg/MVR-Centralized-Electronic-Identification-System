export interface IAppConfig {
    oauth: {
        issuer: string;
        redirectUri: string;
        responseType: string;
        scope: string;
        accessTokenKey: string;
        refreshTokenKey: string;
        tokenEndpoint: string;
        logoutUrl: string;
        realm: string;
        updateMinValidity: number;
    };
    externalLinks: {
        providersApi: string
    }
}
