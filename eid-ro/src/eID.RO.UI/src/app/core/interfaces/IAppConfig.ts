export interface IAppConfig {
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
