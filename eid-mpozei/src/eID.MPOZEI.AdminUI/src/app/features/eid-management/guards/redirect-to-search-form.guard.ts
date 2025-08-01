import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root',
})
export class RedirectToSearchFormGuard {
    constructor(private router: Router) {}

    canActivate() {
        const navigationExtras = this.router.getCurrentNavigation()?.extras;
        if (navigationExtras && navigationExtras.state) {
            return true;
        } else {
            return this.router.parseUrl('/eid-management');
        }
    }
}
