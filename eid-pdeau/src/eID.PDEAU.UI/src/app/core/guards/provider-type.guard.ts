import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CurrentProviderService } from '@app/core/services/current-provider.service';
import { DetailedProviderType } from '@app/features/providers/provider.dto';
import { Observable } from 'rxjs';

export const ProviderTypeGuard = (type: DetailedProviderType['type']): CanActivateFn => {
    return () => {
        const currentProviderService = inject(CurrentProviderService);
        const router = inject(Router);

        if (!currentProviderService.getProvider()) {
            return new Observable<boolean>(observer => {
                currentProviderService.init(() => {
                    observer.next(currentProviderService.isOfType(type));
                    observer.complete();
                });
            });
        } else if (currentProviderService.isOfType(type)) {
            return true;
        } else {
            return router.parseUrl('/unauthorized');
        }
    };
};
