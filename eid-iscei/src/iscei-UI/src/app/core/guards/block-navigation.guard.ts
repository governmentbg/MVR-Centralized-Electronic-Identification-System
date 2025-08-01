import { inject } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationDialogService } from '../../shared/services/confirmation-dialog.service';
import { Observable } from 'rxjs';

export const blockNavigationGuard = (component: any): Observable<boolean> => {
    const confirmationDialogService = inject(ConfirmationDialogService);
    const translateService = inject(TranslocoService);

    return new Observable<boolean>(observer => {
        if (component.unsavedFormDataExists) {
            confirmationDialogService.showConfirmation({
                header: translateService.translate('global.txtConfirmation'),
                message: translateService.translate('global.txtUnsavedChangesConfirmation'),
                accept: () => {
                    observer.next(true);
                    observer.complete();
                },
                reject: () => {
                    observer.next(false);
                    observer.complete();
                },
            });
        } else {
            observer.next(true);
            observer.complete();
        }
    });
};
