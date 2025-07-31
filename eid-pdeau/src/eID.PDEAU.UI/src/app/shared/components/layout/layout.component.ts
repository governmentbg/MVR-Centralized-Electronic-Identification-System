import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { AlertService } from '@app/shared/services/alert.service';

@Component({
    selector: 'app-layout',
    templateUrl: './layout.component.html',
    styleUrls: ['./layout.component.scss'],
})
export class LayoutComponent {
    constructor(private oauthService: OAuthService, private alertService: AlertService) {}
    _alertService = this.alertService;

    get hasValidAccessToken() {
        return this.oauthService.hasValidAccessToken();
    }
}
