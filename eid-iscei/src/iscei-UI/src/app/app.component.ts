import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { OAuthService, OAuthEvent, OAuthErrorEvent } from 'angular-oauth2-oidc';
import { AuthService } from './core/services/auth.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnDestroy {
    private oauthSubscription: Subscription = new Subscription();

    constructor(private oauthService: OAuthService, private authService: AuthService) {
        this.oauthSubscription.add(
            this.oauthService.events.subscribe((event: OAuthEvent | OAuthErrorEvent) => {
                if (event.type === 'token_expires') {
                    // Handle token expiration event
                    this.authService.removeTokenData();
                    this.oauthService.revokeTokenAndLogout();
                }
            })
        );
    }

    title = 'eID.ISCEI.UI';

    ngOnDestroy() {
        this.oauthSubscription.unsubscribe();
    }
}
