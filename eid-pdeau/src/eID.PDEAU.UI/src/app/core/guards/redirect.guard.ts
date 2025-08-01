import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { RoleType, SystemType } from '../enums/auth.enum';
import { UserService } from '../services/user.service';

export const redirectGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const userService = inject(UserService);

    switch (userService.getSystemType()) {
        case SystemType.DEAU:
            if (userService.hasRole(RoleType.ADMINISTRATOR)) {
                router.navigate(['providers/current']);
                return false;
            } else if (userService.hasRole(RoleType.EMPLOYEE)) {
                router.navigate(['empowerment-check']);
                return false;
            } else {
                router.navigate(['/unauthorized']);
                return false;
            }
        case SystemType.AEI:
            if (userService.hasRole(RoleType.ADMINISTRATOR) || userService.hasRole(RoleType.EMPLOYEE)) {
                router.navigate(['administrators']);
                return false;
            } else {
                router.navigate(['/unauthorized']);
                return false;
            }
        case SystemType.CEI:
            if (userService.hasRole(RoleType.ADMINISTRATOR) || userService.hasRole(RoleType.EMPLOYEE)) {
                router.navigate(['centers']);
                return false;
            } else {
                router.navigate(['/unauthorized']);
                return false;
            }
        default:
            router.navigate(['/administrators']);
            return false;
    }
};
