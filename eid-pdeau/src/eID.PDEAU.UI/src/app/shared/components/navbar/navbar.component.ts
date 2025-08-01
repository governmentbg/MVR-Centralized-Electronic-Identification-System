import { Component } from '@angular/core';
import { AuthService } from '@app/core/services/auth.service';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '@app/core/services/user.service';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
    constructor(
        private oAuthService: OAuthService,
        private authService: AuthService,
        private userService: UserService
    ) {}

    get user() {
        return this.userService.getUser();
    }

    logout() {
        this.authService.removeTokenData();
        this.oAuthService.logOut();
    }

    get hasValidAccessToken() {
        return this.oAuthService.hasValidAccessToken();
    }
}
