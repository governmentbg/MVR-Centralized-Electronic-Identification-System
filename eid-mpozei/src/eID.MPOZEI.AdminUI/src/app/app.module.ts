import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TranslocoRootModule } from './transloco-root.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslocoService } from '@ngneat/transloco';
import { lastValueFrom } from 'rxjs';
import { AppComponent } from './app.component';
import { HttpCancelService } from './core/services/httpcancel.service';
import { ManageHttpInterceptor } from './core/interceptors/managehttp.interceptor';
import { AppConfigService } from './core/services/config.service';
import { EidDeviceService } from './features/eid-management/services/eid-device.service';
import { AppConfigService as AppLibConfigService, NgxEidLogsViewerModule } from '@eid/ngx-eid-logs-viewer';
import { EidAdministratorService } from './features/eid-management/services/eid-administrator.service';
import jwt_decode from 'jwt-decode';
import { OAuthModule, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { UserService } from './core/services/user.service';

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
    eidDeviceService: EidDeviceService,
    eidAdministratorService: EidAdministratorService
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
                useSilentRefresh: res.oauth.useSilentRefresh,
                timeoutFactor: res.oauth.timeoutFactor, // 0.75 = Refresh the token when % of the token's lifetime is over
            });

            oAuthService
                .loadDiscoveryDocumentAndLogin({ preventClearHashAfterLogin: true })
                .then(async isLoggedIn => {
                    if (isLoggedIn) {
                        await handleUserLogin(oAuthService, userService, eidDeviceService, eidAdministratorService);
                        resolve(true);
                    } else {
                        oAuthService.initCodeFlow();
                    }
                })
                .catch(error => {
                    console.error(error);
                    oAuthService.initLoginFlow();
                });

            oAuthService.setupAutomaticSilentRefresh();
            oAuthService.tokenValidationHandler = new JwksValidationHandler();
        });
    });
}

function preloadDevices(eidDeviceService: EidDeviceService) {
    return new Promise<any>(resolve => {
        eidDeviceService.loadDevices().subscribe({
            next: () => {
                resolve(true);
            },
            error: () => {
                resolve(true);
            },
        });
    });
}
function preloadAdministrators(
    eidAdministratorService: EidAdministratorService,
    currentAdministratorId: string,
    eidAdministratorFrontOfficeId: string
) {
    return new Promise<any>(resolve => {
        eidAdministratorService.loadAdministrators().subscribe({
            next: () => {
                eidAdministratorService.currentAdministratorId = currentAdministratorId;
                eidAdministratorService.currentAdministratorOfficeId = eidAdministratorFrontOfficeId;
                resolve(true);
            },
            error: () => {
                resolve(true);
            },
        });
    });
}

async function handleUserLogin(
    oAuthService: OAuthService,
    userService: UserService,
    eidDeviceService: EidDeviceService,
    eidAdministratorService: EidAdministratorService
) {
    const user = await oAuthService.loadUserProfile();
    userService.user = (user as any).info;
    const token = oAuthService.getAccessToken() || '';
    const decodedToken: any = jwt_decode(token);
    await preloadDevices(eidDeviceService);
    await preloadAdministrators(
        eidAdministratorService,
        decodedToken.eidadministratorid,
        decodedToken.eidadminfrontofficeid
    );
}

export function oAuthStorageFactory(): OAuthStorage {
    return sessionStorage;
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
        { provide: OAuthStorage, useFactory: oAuthStorageFactory },
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [TranslocoService],
            useFactory: preloadUser,
        },
        {
            provide: APP_INITIALIZER,
            multi: true,
            deps: [
                AppConfigService,
                AppLibConfigService,
                OAuthService,
                UserService,
                EidDeviceService,
                EidAdministratorService,
            ],
            useFactory: (
                appConfigService: AppConfigService,
                appLibConfigService: AppLibConfigService,
                oAuthService: OAuthService,
                userService: UserService,
                eidDeviceService: EidDeviceService,
                eidAdministratorService: EidAdministratorService
            ) => {
                return () => {
                    return initConfig(
                        appConfigService,
                        appLibConfigService,
                        oAuthService,
                        userService,
                        eidDeviceService,
                        eidAdministratorService
                    );
                };
            },
        },
        HttpCancelService,
        { provide: HTTP_INTERCEPTORS, useClass: ManageHttpInterceptor, multi: true },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
