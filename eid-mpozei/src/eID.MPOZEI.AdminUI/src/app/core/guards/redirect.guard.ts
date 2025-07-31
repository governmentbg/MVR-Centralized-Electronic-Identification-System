import { Router } from '@angular/router';
import { inject } from '@angular/core';
import { RoleType } from '../enums/auth.enum';
import { UserService } from '../services/user.service';

export const redirectGuard = () => {
    const router = inject(Router);
    const userService = inject(UserService);

    if (
        userService.hasRole(RoleType.OPERATOR) ||
        userService.hasRole(RoleType.RU_MVR_ADMINISTRATOR) ||
        userService.hasRole(RoleType.AEID)
    ) {
        router.navigate(['eid-management/search']);
    } else if (userService.hasRole(RoleType.APP_ADMINISTRATOR)) {
        router.navigate(['eid-management/applications-list']);
    } else if (userService.hasRole(RoleType.AUDIT_ADMIN)) {
        router.navigate(['/audit-logs']);
    } else {
        router.navigate(['/unauthorized']);
    }
};
