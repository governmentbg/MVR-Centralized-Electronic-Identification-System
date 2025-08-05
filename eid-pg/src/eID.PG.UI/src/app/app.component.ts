import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { OAuthService, OAuthEvent, OAuthErrorEvent } from 'angular-oauth2-oidc';
import { AuthService } from './core/services/auth.service';
import { AuthenticationService } from './authentication/authentication-service/authentication-service.service';
import { AppConfigService } from './core/services/config.service';
import jwt_decode from 'jwt-decode';
import { UserService } from './core/services/user.service';
@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
    private subscription: Subscription = new Subscription();
    private oauthSubscription: Subscription = new Subscription();

    constructor(
        private primengConfig: PrimeNGConfig,
        public translateService: TranslocoService,
        private oauthService: OAuthService,
        private authService: AuthService,
        private authenticationService: AuthenticationService,
        private appConfigService: AppConfigService,
        private userService: UserService
    ) {
        this.subscription.add(
            translateService.langChanges$.subscribe(() => {
                this.primengConfig.setTranslation(this.translateService.translateObject('primeng'));
            })
        );
        this.oauthSubscription.add(
            this.oauthService.events.subscribe((event: OAuthEvent | OAuthErrorEvent) => {
                if (event.type === 'token_expires') {
                    this.authenticationService.refreshToken(this.appConfigService.config.oauth.clientId).subscribe({
                        next: async res => {
                            this.authService.addTokenData(res.access_token, res.refresh_token);
                            this.userService.setUser(jwt_decode(res.access_token));
                            const user = this.userService.getUser();
                            sessionStorage.setItem('user', JSON.stringify(user));
                            this.oauthService.initCodeFlow();
                            this.userService.userSubject.next(true);
                        },
                        error: () => {
                            this.authService.removeTokenData();
                            this.oauthService.revokeTokenAndLogout();
                        },
                    });
                }
            })
        );
    }

    ngOnInit() {
        this.primengConfig.filterMatchModeOptions = {
            text: [
                FilterMatchMode.STARTS_WITH,
                FilterMatchMode.CONTAINS,
                FilterMatchMode.NOT_CONTAINS,
                FilterMatchMode.ENDS_WITH,
                FilterMatchMode.EQUALS,
                FilterMatchMode.NOT_EQUALS,
            ],
            numeric: [
                FilterMatchMode.EQUALS,
                FilterMatchMode.NOT_EQUALS,
                FilterMatchMode.LESS_THAN,
                FilterMatchMode.LESS_THAN_OR_EQUAL_TO,
                FilterMatchMode.GREATER_THAN,
                FilterMatchMode.GREATER_THAN_OR_EQUAL_TO,
            ],
            date: [
                FilterMatchMode.DATE_IS,
                FilterMatchMode.DATE_IS_NOT,
                FilterMatchMode.DATE_BEFORE,
                FilterMatchMode.DATE_AFTER,
            ],
        };

        const activeLanguage = this.translateService.getActiveLang();
        this.translateService.load(activeLanguage).subscribe(() => {
            this.primengConfig.setTranslation(this.translateService.translateObject('primeng'));
        });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.oauthSubscription.unsubscribe();
    }
}
