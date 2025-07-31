import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root',
})
export class RedirectToJournalGuard {
    constructor(private router: Router) {}

    canActivate() {
        const navigationExtras = this.router.getCurrentNavigation()?.extras;
        if (navigationExtras && navigationExtras.state) {
            return true;
        } else {
            return this.router.parseUrl('/journal');
        }
    }
}
