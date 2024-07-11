import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { TranslocoRootModule } from './transloco-root.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { InputSwitchModule } from 'primeng/inputswitch';
import { APP_INITIALIZER } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { lastValueFrom } from 'rxjs';
import { KeycloakAngularModule, KeycloakService } from 'keycloak-angular';
import { AuthService } from './core/services/auth.service';
import { AppConfigService } from './core/services/config.service';

export function preloadUser(transloco: TranslocoService) {
    return function () {
        transloco.setActiveLang('bg');
        return lastValueFrom(transloco.load('bg'));
    };
}

function initializeKeycloak(
    keycloakService: KeycloakService,
    appConfigService: AppConfigService,
    authService: AuthService
) {
    return keycloakService.init({
        config: {
            url: appConfigService.config.keycloak.keycloakUrl,
            realm: appConfigService.config.keycloak.keycloakRealm,
            clientId: appConfigService.config.keycloak.keycloakClientId,
        },
        initOptions: {
            // must match to the configured value in keycloak
            redirectUri: appConfigService.config.keycloak.keycloakRedirectUri,
            onLoad: 'login-required',
            token: authService.getTokenData().token,
            refreshToken: authService.getTokenData().refreshToken,
            checkLoginIframe: false,
        },
        updateMinValidity: appConfigService.config.keycloak.keycloakUpdateMinValidity,
        shouldUpdateToken() {
            return true;
        },
        bearerExcludedUrls: ['/assets', '/clients/public'],
    });
}

function initConfig(
    appConfigService: AppConfigService,
    keycloakService: KeycloakService,
    authService: AuthService
): Promise<any> {
    return new Promise<any>(resolve => {
        appConfigService.loadAppConfig().then(res => {
            initializeKeycloak(keycloakService, appConfigService, authService).then(success => {
                if (success) {
                    const token = keycloakService.getKeycloakInstance().token || '';
                    const refreshToken = keycloakService.getKeycloakInstance().refreshToken || '';
                    authService.addTokenData(token, refreshToken);
                    resolve(true);
                } else {
                    resolve(true);
                }
            });
        });
    });
}

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        HttpClientModule,
        TranslocoRootModule,
        CoreModule,
        SharedModule,
        FormsModule,
        ReactiveFormsModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        InputSwitchModule,
        KeycloakAngularModule,
    ],
    providers: [
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [TranslocoService],
            useFactory: preloadUser,
        },
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [AppConfigService, KeycloakService, AuthService],
            useFactory: (
                appConfigService: AppConfigService,
                keycloakService: KeycloakService,
                authService: AuthService
            ) => {
                return () => {
                    return initConfig(appConfigService, keycloakService, authService);
                };
            },
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
