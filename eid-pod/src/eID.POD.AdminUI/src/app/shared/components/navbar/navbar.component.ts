import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';
import { AuthService } from '../../../core/services/auth.service';
@Component({
    selector: 'app-navbar',
    templateUrl: './navbar.component.html',
    styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
    constructor(private router: Router, private keycloak: KeycloakService, private authService: AuthService) {}

    logout() {
        localStorage.removeItem('isLoggedIn');
        this.router.navigate(['/login']);
        this.authService.removeTokenData();
        this.keycloak.logout();
    }
}
