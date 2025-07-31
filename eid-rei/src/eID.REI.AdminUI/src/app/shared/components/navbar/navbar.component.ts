import { Component } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { UserService } from '../../../core/services/user.service';

@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
    constructor(private oAuthService: OAuthService, private userService: UserService) {}

    get name() {
        return this.userService.user.name;
    }

    logout() {
        this.oAuthService.revokeTokenAndLogout();
    }
}
