import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { lastValueFrom } from 'rxjs';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TranslocoRootModule } from './transloco-root.module';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { OAuthModule, OAuthService, OAuthStorage } from 'angular-oauth2-oidc';
import { JwksValidationHandler } from 'angular-oauth2-oidc-jwks';
import { TranslocoService } from '@ngneat/transloco';
import { AuthInterceptor } from './core/interceptors/http-requests.interceptor';
import { AppConfigService } from './core/services/config.service';

export function preloadUser(transloco: TranslocoService) {
    return () => {
        transloco.setActiveLang('bg');
        return lastValueFrom(transloco.load('bg'));
    };
}

export function initAuthService(oauthService: OAuthService, appConfigService: AppConfigService): () => Promise<any> {
    return () => {
        return appConfigService.loadAppConfig().then(res => {
            const token = oauthService.hasValidAccessToken();
            if (!token) {
                oauthService.configure(res.oauth);
                oauthService.setupAutomaticSilentRefresh({
                    params: { client_id: res.oauth.clientId, grant_type: 'REFRESH_TOKEN' },
                });
                oauthService.tokenValidationHandler = new JwksValidationHandler();
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
        ButtonModule,
        CardModule,
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
            provide: APP_INITIALIZER,
            deps: [OAuthService, AppConfigService],
            multi: true,
            useFactory: initAuthService,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        },
    ],
    bootstrap: [AppComponent],
})
export class AppModule {}
