import { Component, OnDestroy, OnInit } from '@angular/core';
import { FilterMatchMode, PrimeNGConfig } from 'primeng/api';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription } from 'rxjs';
import { KeycloakEventType, KeycloakService } from 'keycloak-angular';
import { AuthService } from './core/services/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
    constructor(
        private primengConfig: PrimeNGConfig,
        public translateService: TranslocoService,
        private keycloakService: KeycloakService,
        private authService: AuthService
    ) {
        this.subscription = translateService.langChanges$.subscribe(() => {
            this.primengConfig.setTranslation(this.translateService.translateObject('primeng'));
        });

        this.keycloakSubscription = keycloakService.keycloakEvents$.subscribe({
            next(event) {
                if (event.type == KeycloakEventType.OnTokenExpired) {
                    authService.removeTokenData();
                    keycloakService.logout();
                }
            },
        });
    }

    subscription: Subscription;
    keycloakSubscription: Subscription;
    title = 'eID.PAN.AdminUI';

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
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.keycloakSubscription.unsubscribe();
    }
}
