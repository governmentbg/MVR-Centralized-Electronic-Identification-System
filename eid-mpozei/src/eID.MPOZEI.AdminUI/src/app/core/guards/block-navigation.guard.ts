import { inject } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationDialogService } from '../../shared/services/confirmation-dialog.service';
import { Observable } from 'rxjs';
import { ActivatedRouteSnapshot } from '@angular/router';
export const blockNavigationGuard = (component: any, currentRoute: ActivatedRouteSnapshot): Observable<boolean> => {
    const confirmationDialogService = inject(ConfirmationDialogService);
    const translateService = inject(TranslocoService);
    return new Observable<boolean>(observer => {
        if (component.unsavedFormDataExists) {
            let message = translateService.translate('global.txtUnsavedChangesConfirmation');
            if (currentRoute.data['blockNavigationGuardMessage']) {
                message = translateService.translate(currentRoute.data['blockNavigationGuardMessage']);
            }

            confirmationDialogService.showConfirmation({
                header: translateService.translate('global.txtConfirmation'),
                message: message,
                accept: () => {
                    observer.next(true);
                    observer.complete();
                },
                reject: () => {
                    observer.next(false);
                    observer.complete();
                },
            });
        } else if (component.exportCsvRunning) {
            confirmationDialogService.showConfirmation({
                header: translateService.translate('global.txtConfirmation'),
                message: translateService.translate('modules.logsViewer.txtNavigateDuringExportCSV'),
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
