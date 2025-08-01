import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '@app/core/services/user.service';
import { RoleType } from '@app/core/enums/auth.enum';

export const providerGuard = () => {
    const router = inject(Router);
    const userService = inject(UserService);

    if (userService.hasRole(RoleType.ADMINISTRATOR)) {
        router.navigate(['providers/current']);
    } else if (userService.hasRole(RoleType.EMPLOYEE)) {
        router.navigate(['empowerment-check']);
    } else {
        router.navigate(['providers/applications']);
    }
};
