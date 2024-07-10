import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';

@Component({
    selector: 'app-empowerment-check-navbar',
    templateUrl: './empowerment-check-navbar.component.html',
    styleUrls: ['./empowerment-check-navbar.component.scss'],
})
export class EmpowermentCheckNavbarComponent {
    constructor(private router: Router, private keycloak: KeycloakService) {}

    logout() {
        // this.authService.removeTokenData();
        this.keycloak.logout();
    }
}
