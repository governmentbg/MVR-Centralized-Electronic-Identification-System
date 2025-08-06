import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Location } from '@angular/common';

export const mobileRedirectGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const location = inject(Location);
    const previousUrl = location.path();
    const newUrl = state.url;

    if (previousUrl.includes('mobile') && !newUrl.includes('mobile')) {
        router.navigate([`/mobile/${newUrl}`]);
        return false;
    }
    return true;
};
