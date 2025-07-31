export interface IAppConfig {
    oauth: {
        issuer: string;
        clientId: string;
        redirectUri: string;
        requireHttps: boolean;
        responseType: string;
        scope: string;
        authUrl: string;
        accessTokenKey: string;
        refreshTokenKey: string;
        tokenEndpoint: string;
        provider: string;
        logoutUrl: string;
        realm: string;
        updateMinValidity: number;
    };
    uiFeatures: {
        usefulInformationUrl: string;
    };
}
