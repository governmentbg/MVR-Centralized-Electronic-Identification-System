import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { TranslocoRootModule } from './transloco-root.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_INITIALIZER } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { lastValueFrom } from 'rxjs';
import { HomeComponent } from './pages/home/home.component';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { AppConfigService } from './core/services/config.service';
import { GeneralInformationModule } from './features/general-information/general-information.module';
import { OAuthModule, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { AuthInterceptor } from '@app/core/interceptors/http-requests.interceptor';
import { UserService } from '@app/core/services/user.service';

export function preloadUser(transloco: TranslocoService) {
    return function () {
        transloco.setActiveLang('bg');
        return lastValueFrom(transloco.load('bg'));
    };
}

export function initAuthService(
    oauthService: OAuthService,
    appConfigService: AppConfigService,
    userService: UserService
): () => Promise<any> {
    return () => {
        return appConfigService.loadAppConfig().then(res => {
            const token = oauthService.hasValidAccessToken();
            if (!token) {
                oauthService.configure(res.oauth);
                oauthService.setupAutomaticSilentRefresh({
                    params: { client_id: res.oauth.clientId, grant_type: 'REFRESH_TOKEN' },
                });
                oauthService.tokenValidationHandler = new JwksValidationHandler();
            } else {
                userService.initSystemTypeForUser();
            }
            oauthService.logoutUrl = `${window.location.origin}/login`;
            oauthService.requireHttps = res.oauth.requireHttps;
            return true;
        });
    };
}

export function oAuthStorageFactory(): OAuthStorage {
    return localStorage;
}

@NgModule({
    declarations: [AppComponent, HomeComponent],
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
        ButtonModule,
        ToastModule,
        GeneralInformationModule,
        OAuthModule.forRoot(),
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
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        },
        {
            provide: APP_INITIALIZER,
            deps: [OAuthService, AppConfigService, UserService],
            multi: true,
            useFactory: initAuthService,
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
