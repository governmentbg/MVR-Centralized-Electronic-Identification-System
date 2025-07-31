import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { OAuthErrorEvent, OAuthEvent, OAuthService } from 'angular-oauth2-oidc';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
    constructor(
        private primengConfig: PrimeNGConfig,
        public translateService: TranslocoService,
        private oAuthService: OAuthService
    ) {
        this.subscription = translateService.langChanges$.subscribe(() => {
            this.primengConfig.setTranslation(this.translateService.translateObject('primeng'));
        });

        this.oauthSubscription.add(
            this.oAuthService.events.subscribe((event: OAuthEvent | OAuthErrorEvent) => {
                if (event.type === 'token_expires') {
                    // Handle token expiration event
                    this.oauthSubscription.unsubscribe();
                    this.oAuthService.revokeTokenAndLogout();
                }
            })
        );
    }

    subscription: Subscription;
    oauthSubscription: Subscription = new Subscription();
    title = 'eID.RO.AdminUI';

    ngOnInit() {
        // Just a demonstration of how we can change the filter options of the table
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
