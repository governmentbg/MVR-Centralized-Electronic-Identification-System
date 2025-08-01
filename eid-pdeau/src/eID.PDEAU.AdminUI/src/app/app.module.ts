import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';
import { TranslocoRootModule } from './transloco-root.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslocoService } from '@ngneat/transloco';
import { firstValueFrom, lastValueFrom } from 'rxjs';
import { AppComponent } from './app.component';
import { AppConfigService } from './core/services/config.service';
import { AppConfigService as AppLibConfigService, NgxEidLogsViewerModule } from '@eid/ngx-eid-logs-viewer';
import { OAuthModule, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { UserService } from './core/services/user.service';
import { ProvidedServicesService } from '@app/features/tariffs/services/provided-services.service';

export function preloadUser(transloco: TranslocoService) {
    return function () {
        let activeLang = 'bg';
        const localStorageActiveLang = localStorage.getItem('activeLang');
        if (localStorageActiveLang !== null) {
            activeLang = localStorageActiveLang;
        }
        localStorage.setItem('activeLang', activeLang);
        transloco.setActiveLang(activeLang);
        return lastValueFrom(transloco.load(activeLang));
    };
}

function initConfig(
    appConfigService: AppConfigService,
    appLibConfigService: AppLibConfigService,
    oAuthService: OAuthService,
    userService: UserService,
    providedServices: ProvidedServicesService
): Promise<any> {
    return new Promise<any>(resolve => {
        appConfigService.loadAppConfig().then(async res => {
            appLibConfigService.setConfig(res as any);
            oAuthService.configure({
                clientId: res.oauth.clientId,
                issuer: res.oauth.issuer,
                redirectUri: res.oauth.redirectUri,
                responseType: res.oauth.responseType,
                requireHttps: res.oauth.requireHttps,
                scope: res.oauth.scope,
                preserveRequestedRoute: true,
                useSilentRefresh: res.oauth.useSilentRefresh,
                timeoutFactor: res.oauth.timeoutFactor, // 0.75 = Refresh the token when % of the token's lifetime is over
            });
            oAuthService.setupAutomaticSilentRefresh();
            oAuthService.tokenValidationHandler = new JwksValidationHandler();

            oAuthService
                .loadDiscoveryDocumentAndLogin({ preventClearHashAfterLogin: true })
                .then(async isLoggedIn => {
                    if (isLoggedIn) {
                        await handleUserLogin(oAuthService, userService, providedServices);
                        resolve(true);
                    } else {
                        oAuthService.initCodeFlow();
                    }
                })
                .catch(error => {
                    console.error(error);
                    oAuthService.revokeTokenAndLogout();
                });
        });
    });
}

async function handleUserLogin(
    oAuthService: OAuthService,
    userService: UserService,
    providedServices: ProvidedServicesService
) {
    const user = await oAuthService.loadUserProfile();
    userService.user = (user as any).info;
    await preloadProvidedServices(providedServices);
}

function preloadProvidedServices(providedServices: ProvidedServicesService) {
    return new Promise<any>(resolve => {
        providedServices.loadProvidedServices().subscribe({
            next: () => {
                resolve(true);
            },
            error: () => {
                resolve(true);
            },
        });
    });
}

export function oAuthStorageFactory(): OAuthStorage {
    return localStorage;
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
        NgxEidLogsViewerModule,
        OAuthModule.forRoot({
            resourceServer: {
                sendAccessToken: true,
            },
        }),
    ],
    providers: [
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [TranslocoService],
            useFactory: preloadUser,
        },
        { provide: OAuthStorage, useFactory: oAuthStorageFactory },
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [AppConfigService, AppLibConfigService, OAuthService, UserService, ProvidedServicesService],
            useFactory: (
                appConfigService: AppConfigService,
                appLibConfigService: AppLibConfigService,
                oAuthService: OAuthService,
                userService: UserService,
                providedServices: ProvidedServicesService
            ) => {
                return () => {
                    return initConfig(
                        appConfigService,
                        appLibConfigService,
                        oAuthService,
                        userService,
                        providedServices
                    );
                };
            },
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
